using KwikNesta.Gateway.Svc.API.Grpc.Identity;
using KwikNesta.Gateway.Svc.API.Services.Interfaces;

namespace KwikNesta.Gateway.Svc.API.Services
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IGrpcAuthenticationServiceImpl> _grpcAuthenticationServiceImpl;
        private readonly Lazy<IGrpcUserServiceImpl> _grpcUserServiceImpl;

        public ServiceManager(AuthenticationService.AuthenticationServiceClient authService,
                              AppUserService.AppUserServiceClient userService) 
        {
            _grpcAuthenticationServiceImpl = new Lazy<IGrpcAuthenticationServiceImpl>(()
                => new GrpcAuthenticationServiceImpl(authService));
            _grpcUserServiceImpl = new Lazy<IGrpcUserServiceImpl>(() 
                => new GrpcUserServiceImpl(userService));
        }

        public IGrpcAuthenticationServiceImpl Authentication => _grpcAuthenticationServiceImpl.Value;
        public IGrpcUserServiceImpl User => _grpcUserServiceImpl.Value;
    }
}
