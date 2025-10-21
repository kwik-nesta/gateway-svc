namespace KwikNesta.Gateway.Svc.API.Extensions
{
    public class ForwardAuthHeaderHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ForwardAuthHeaderHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var context = _httpContextAccessor.HttpContext;

            if (context?.Request.Headers.TryGetValue("Authorization", out var authHeader) ?? false)
            {
                request.Headers.TryAddWithoutValidation("Authorization", authHeader.ToString());
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}