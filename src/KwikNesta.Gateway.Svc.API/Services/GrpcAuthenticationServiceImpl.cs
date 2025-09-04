using KwikNesta.Gateway.Svc.API.Grpc.Identity;
using KwikNesta.Gateway.Svc.API.Services.Interfaces;

namespace KwikNesta.Gateway.Svc.API.Services
{
    public class GrpcAuthenticationServiceImpl : IGrpcAuthenticationServiceImpl
    {
        private readonly AuthenticationService.AuthenticationServiceClient _service;

        public GrpcAuthenticationServiceImpl(AuthenticationService.AuthenticationServiceClient service) 
            => _service = service;

        public async Task<TokenResponse> LoginAsync(LoginRequest request)
        {
            var loginResult = await _service.LoginAsync(request);
            return loginResult.Tokens;
        }
    }
}
