using CrossQueue.Hub.Shared.Extensions;
using DiagnosKit.Core.Extensions;
using DRY.MailJetClient.Library.Extensions;
using EFCore.CrudKit.Library.Extensions;
using Hangfire;
using Hangfire.Console;
using Hangfire.PostgreSql;
using KwikNesta.Contracts.Models;
using KwikNesta.Gateway.Svc.API.Grpc.Identity;
using KwikNesta.Gateway.Svc.API.Settings;
using KwikNesta.Gateway.Svc.Application.Interfaces;
using KwikNesta.Gateway.Svc.Infrastructure.Jobs;
using KwikNesta.Gateway.Svc.Infrastructure.Persistence;
using KwikNesta.Gateway.Svc.Infrastructure.Workers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Refit;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

namespace KwikNesta.Gateway.Svc.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services,
                                                          IConfiguration configuration,
                                                          string serviceName)
        {
            services.AddCors(o =>
            {
                o.AddPolicy("Frontends", p => p
                    .WithOrigins(configuration.GetSection("Cors:Origins").Get<string[]>() ?? [])
                    .AllowAnyHeader()
                    .AllowAnyMethod());
            })
            .RegisterDbContext(configuration)
            .RegisterWorkers()
            .AddScoped<IMessageProcessor, MessageProcessor>()
            .ConfigureHangfire(configuration)
            .AddJwtAuth(configuration)
            .AddThrotter()
            .AddSwaggerDocs()
            .AddCrossQueueHubRabbitMqBus(configuration)
            .AddApiVersion()
            .ConfigureRefit(configuration)
            .RegisterGrpcClients(configuration)
            .AddDiagnosKitObservability(serviceName: serviceName, serviceVersion: "1.0.0")
            .ConfigureMailJet(configuration)
            .AddLoggerManager();
            services
                .ConfigureEFCoreDataForge<SupportDbContext>(false);

            return services;
        }

        private static IServiceCollection ConfigureHangfire(this IServiceCollection services,
                                                           IConfiguration configuration)
        {
            services.AddHangfire(config =>
            {
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UsePostgreSqlStorage(opt =>
                    {
                        opt.UseNpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
                    })
                    .UseConsole()
                    .UseFilter(new AutomaticRetryAttribute()
                    {
                        Attempts = 5,
                        DelayInSecondsByAttemptFunc = _ => 60
                    });
            }).AddHangfireServer(opt =>
            {
                opt.ServerName = "Kwik Nesta Hangfire Server";
                opt.Queues = new[] { "recurring", "default" };
                opt.SchedulePollingInterval = TimeSpan.FromMinutes(1);
                opt.WorkerCount = 5;
            });

            return services;
        }

        private static IServiceCollection RegisterDbContext(this IServiceCollection services,
                                                            IConfiguration configuration)
        {
            services.AddDbContext<SupportDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            return services;
        }

        private static IServiceCollection RegisterWorkers(this IServiceCollection services)
        {
            services.AddHostedService<NotificationWorker>()
               .AddHostedService<AuditWorker>()
               .AddHostedService<DataloadWorker>();

            return services;
        }

        private static IServiceCollection ConfigureRefit(this IServiceCollection services,
                                                        IConfiguration configuration)
        {
            services.AddHttpContextAccessor();

            services.AddTransient<ForwardAuthHeaderHandler>();

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };

            // Refit settings using System.Text.Json with the options above
            var refitSettings = new RefitSettings
            {
                ContentSerializer = new SystemTextJsonContentSerializer(options)
            };

            var servers = configuration.GetSection("KwikNestaServers").Get<KwikNestaServers>() ??
                throw new ArgumentNullException("KwikNestaServers section is null");

            services.AddRefitClient<IIdentityServiceClient>(refitSettings)
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(servers.IdentityService))
                .AddHttpMessageHandler<ForwardAuthHeaderHandler>();

            services.AddRefitClient<ILocationClientService>(refitSettings)
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(servers.ExternalLocationClient));

            return services;
        }

        private static IServiceCollection RegisterGrpcClients(this IServiceCollection services,
                                                              IConfiguration configuration)
        {
            var grpcServers = configuration.GetSection("KwikNestaServers").Get<KwikNestaServers>() ??
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
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
                c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);

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
                    Version = "v2",
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
                        ValidIssuer = jwtSettings.Issuer,

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