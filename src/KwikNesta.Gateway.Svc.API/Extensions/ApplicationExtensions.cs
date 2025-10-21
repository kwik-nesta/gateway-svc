using DiagnosKit.Core.Extensions;
using Hangfire;
using KwikNesta.Contracts.Filters;
using KwikNesta.Contracts.Settings;
using KwikNesta.Gateway.Svc.Application.Services;
using KwikNesta.Gateway.Svc.Application.Settings;
using Microsoft.Extensions.Options;

namespace KwikNesta.Gateway.Svc.API.Extensions
{
    public static class ApplicationExtensions
    {
        public static WebApplication UseMiddlewares(this WebApplication app,
                                                    IConfiguration configuration)
        {
            // Security headers first (before anything else touches the response)
            app.UseHsts();
            app.Use((ctx, next) =>
            {
                ctx.Response.Headers.XContentTypeOptions = "nosniff";
                ctx.Response.Headers.XFrameOptions = "DENY";
                ctx.Response.Headers.XXSSProtection = "1; mode=block";
                return next();
            });

            app.UseDiagnosKitPrometheus();

            app.UseSwagger();
            app.UseSwaggerUI(o =>
            {
                o.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                //o.SwaggerEndpoint("/swagger/v2/swagger.json", "v2");
            });

            // HTTPS redirection (enforce HTTPS)
            app.UseHttpsRedirection();

            // CORS (before auth to reject invalid origins early)
            app.UseCors("Frontends");

            // Rate limiting (defend before hitting auth logic)
            app.UseRateLimiter();

            // Authentication (populate HttpContext.User from JWT)
            app.UseAuthentication();

            // Authorization (enforce [Authorize] / policies)
            app.UseAuthorization();

            app.UseHangfireDashboard(configuration);

            app.ScheduleServicePings();

            // Controllers / Endpoints (the actual proxy logic)
            app.MapControllers();

            // Health endpoints (open access for k8s/ops)
            app.MapGet("/", () => Results.Ok(new
            {
                Status = 200,
                Successful = true,
                Message = $"Kwik Nesta Gateway service running in {app.Environment.EnvironmentName} mode..."
            }));

            return app;
        }

        private static WebApplication UseHangfireDashboard(this WebApplication app, IConfiguration configuration)
        {
            var settings = configuration.GetSection("HangfireSettings")
                .Get<HangfireSettings>() ?? throw new ArgumentNullException("HangfireSettings");

            app.UseHangfireDashboard("/admin/jobs", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthFilter(settings) },
                DashboardTitle = "Kwik Nesta Hangfire Dashboard",
                DisplayStorageConnectionString = false,
                DisplayNameFunc = (_, job) => job.Method.Name,
                DarkModeEnabled = true,
            });

            return app;
        }

        private static WebApplication ScheduleServicePings(this WebApplication app)
        {
            var pingJobOptions = app.Services
                .GetRequiredService<IOptions<PingSettings>>().Value;

            RecurringJob.AddOrUpdate<PingService>(
                "ping-all-services",
                job => job.PingAllAsync(null!),
                pingJobOptions.Interval
            );

            return app;
        }
    }
}
