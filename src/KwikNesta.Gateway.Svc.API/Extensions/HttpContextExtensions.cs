using Grpc.Core;

namespace KwikNesta.Gateway.Svc.API.Extensions
{
    public static class HttpContextExtensions
    {
        public static Metadata GetHeaderMeta(this HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();
            var accessToken = authHeader.StartsWith("Bearer ") ? authHeader["Bearer ".Length..] : null;

            var headers = new Metadata
            {
                { "Authorization", $"Bearer {accessToken}" }
            };

            return headers;
        }
    }
}