using DiagnosKit.Core.Extensions;

namespace KwikNesta.Gateway.Svc.API.Extensions
{
    public static class ApplicationExtensions
    {
        public static WebApplication UseMiddlewares(this WebApplication app)
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

            app.UseDiagnosKitPrometheus()
                .UseDiagnosKitLogEnricher();

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

            // Controllers / Endpoints (the actual proxy logic)
            app.MapControllers();

            // Health endpoints (open access for k8s/ops)
            app.MapGet("/", () => Results.Ok("OK"));

            return app;
        }
    }
}
