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

    }
}