using DiagnosKit.Core.Extensions;
using Hangfire;
using KwikNesta.Gateway.Svc.API.Filters;
using KwikNesta.Gateway.Svc.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

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

            app.RunMigrations(true);

            // Controllers / Endpoints (the actual proxy logic)
            app.MapControllers();

            // Health endpoints (open access for k8s/ops)
            app.MapGet("/", () => Results.Ok("OK"));

            return app;
        }

        private static WebApplication UseHangfireDashboard(this WebApplication app, IConfiguration configuration)
        {
            app.UseHangfireDashboard("/admin/jobs", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorizationFilter(configuration) },
                DashboardTitle = "Kwik Nesta Hangfire Dashboard",
                DisplayStorageConnectionString = false,
                DisplayNameFunc = (_, job) => job.Method.Name,
                DarkModeEnabled = true,
            });

            return app;
        }

        internal static WebApplication RunMigrations(this WebApplication app, bool alwayRun = false)
        {
            if (app.Environment.IsDevelopment() || alwayRun)
            {
                using var scope = app.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<SupportDbContext>();
                db.Database.Migrate();
            }

            return app;
        }
    }
}
