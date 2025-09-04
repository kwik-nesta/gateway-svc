using KwikNesta.Gateway.Svc.API.Grpc.Identity;
using KwikNesta.Gateway.Svc.API.Services.Interfaces;

namespace KwikNesta.Gateway.Svc.API.Services
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IGrpcAuthenticationServiceImpl> _grpcAuthenticationServiceImpl;
        public ServiceManager(AuthenticationService.AuthenticationServiceClient authService,
                              AppUserService.AppUserServiceClient userService) 
        {
            _grpcAuthenticationServiceImpl = new Lazy<IGrpcAuthenticationServiceImpl>(()
                => new GrpcAuthenticationServiceImpl(authService));
        }

        public IGrpcAuthenticationServiceImpl Authentication => _grpcAuthenticationServiceImpl.Value;
    }
}
