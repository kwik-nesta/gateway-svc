using KwikNesta.Gateway.Svc.API.Grpc.Identity;

namespace KwikNesta.Gateway.Svc.API.Services.Interfaces
{
    public interface IGrpcAuthenticationServiceImpl
    {
        Task<TokenResponse> LoginAsync(LoginRequest request);
    }
}
