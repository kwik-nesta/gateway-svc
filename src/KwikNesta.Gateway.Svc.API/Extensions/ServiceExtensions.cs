using DiagnosKit.Core.Extensions;
using KwikNesta.Gateway.Svc.API.Grpc.Identity;
using KwikNesta.Gateway.Svc.API.Grpc.SystemSupport;
using KwikNesta.Gateway.Svc.API.Settings;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Threading.RateLimiting;
using KwikNesta.Gateway.Svc.API.Services.Interfaces;
using KwikNesta.Gateway.Svc.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using static System.Collections.Specialized.BitVector32;
using Microsoft.Extensions.Configuration;
using static System.Net.Mime.MediaTypeNames;
using System.Text.Json;

namespace KwikNesta.Gateway.Svc.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services,
                                                          IConfiguration configuration,
                                                          string serviceName)
        {
            services.AddScoped<IServiceManager, ServiceManager>();
            services.AddCors(o =>
            {
                o.AddPolicy("Frontends", p => p
                    .WithOrigins(configuration.GetSection("Cors:Origins").Get<string[]>() ?? [])
                    .AllowAnyHeader()
                    .AllowAnyMethod());
            })
            .AddJwtAuth(configuration)
            .AddThrotter()
            .AddSwaggerDocs()
            .AddApiVersion()
            .AddLoggerManager();
            services.RegisterGrpcClients(configuration)
                .AddDiagnosKitObservability(serviceName: serviceName, serviceVersion: "1.0.0");
            return services;
        }

        private static IServiceCollection RegisterGrpcClients(this IServiceCollection services,
                                                              IConfiguration configuration)
        {
            var grpcServers = configuration.GetSection("GrpcServers").Get<GrpcServers>() ??
                throw new ArgumentNullException("GrpcServer section is null");

            services.AddSingleton(sp => grpcServers);
            services.AddGrpcClient<AuthenticationService.AuthenticationServiceClient>(o =>
            {
                o.Address = new Uri(grpcServers.IdentityService);
            });
            services.AddGrpcClient<AppUserService.AppUserServiceClient>(o =>
            {
                o.Address = new Uri(grpcServers.IdentityService);
            });
            services.AddGrpcClient<LocationService.LocationServiceClient>(o =>
            {
                o.Address = new Uri(grpcServers.SystemSupportService);
            });

            return services;
        }

        private static IServiceCollection AddThrotter(this IServiceCollection services)
        {
            return services.AddRateLimiter(opts =>
            {
                opts.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        ctx.User?.Identity?.Name ?? ctx.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 50,
                            Window = TimeSpan.FromMinutes(1)
                        }));
            });
        }

        private static IServiceCollection AddSwaggerDocs(this IServiceCollection services)
        {
            return services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Kwik Nesta API",
                    Version = "v1",
                    Description = "Kwik Nesta Gateway API v1.0",
                    Contact = new OpenApiContact
                    {
                        Name = "Kwik Nesta Inc.",
                        Email = "info@kwik-nesta.com",
                        Url = new Uri("https://kwik-nesta.com")
                    }
                });
                c.SwaggerDoc("v2", new OpenApiInfo
                {
                    Title = "Kwik Nesta API",
                    Version = "v1",
                    Description = "Kwik Nesta Gateway API v2.0",
                    Contact = new OpenApiContact
                    {
                        Name = "Kwik Nesta Inc.",
                        Email = "info@kwik-nesta.com",
                        Url = new Uri("https://kwik-nesta.com")
                    }
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Kwik Nesta Gateway API"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }});
            });
        }

        private static IServiceCollection AddApiVersion(this IServiceCollection services)
        {
            return services.AddApiVersioning(opt =>
            {
                opt.ReportApiVersions = true;
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                opt.ApiVersionReader = ApiVersionReader.Combine(
                    new HeaderApiVersionReader("api-version"),
                    new HeaderApiVersionReader("X-Version"),
                    new UrlSegmentApiVersionReader());
            });
        }

        private static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt")
                .Get<JwtSetting>() ?? throw new ArgumentNullException("JWT Config can not be null");
            
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.TokenValidationParameters = new()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.IdentityService,

                        ValidateLifetime = true,

                        ValidateAudience = true,
                        ValidAudience = jwtSettings.Audience,

                        RoleClaimType = jwtSettings.RoleClaim,

                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.IssuerSigningKey))
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = async context =>
                        {
                            await ValidateUser(context);
                        },
                        OnChallenge = async context =>
                        {
                            // stop the default behavior (and the WWW-Authenticate header)
                            context.HandleResponse();
                            var statusCode = StatusCodes.Status401Unauthorized;
                            var message = "Unauthorized. Please login";

                            // Check if the failure is due to token expiration
                            if (context.AuthenticateFailure is SecurityTokenExpiredException)
                            {
                                statusCode = StatusCodes.Status403Forbidden;
                                message = "Forbidden. Token has expired!";
                            }

                            context.Response.StatusCode = statusCode;
                            context.Response.ContentType = "application/json";

                            await context.Response.WriteAsJsonAsync(new
                            {
                                Successful = false,
                                Status = statusCode,
                                Message = message
                            });
                        }
                    };
                });

            services.AddAuthorization();
            return services;
        }

        private static async Task ValidateUser(TokenValidatedContext context)
        {
            var service = context.HttpContext.RequestServices.GetRequiredService<AppUserService.AppUserServiceClient>();
            var userId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                context.Fail("Forbidden: Invalid identifier");
                return;
            }

            try
            {
                var userResponse = await service.GetUserByIdAsync(new GetUserByIdRequest
                {
                    UserId = userId
                });
                if (userResponse.User == null || userResponse.User.Status != GrpcUserStatus.Active)
                {
                    context.Fail("Forbidden: User not found");
                    return;
                }
            }
            catch (Exception)
            {
                context.Fail("Forbidden: User not found");
                return;
            }
        }
    }
}