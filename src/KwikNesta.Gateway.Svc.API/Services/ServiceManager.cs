using KwikNesta.Gateway.Svc.API.Grpc.Identity;
using KwikNesta.Gateway.Svc.API.Grpc.SystemSupport;
using KwikNesta.Gateway.Svc.API.Services.Interfaces;
using KwikNesta.SystemSupport.Svc.Contracts;

namespace KwikNesta.Gateway.Svc.API.Services
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IGrpcAuthenticationServiceImpl> _grpcAuthenticationServiceImpl;
        private readonly Lazy<IGrpcUserServiceImpl> _grpcUserServiceImpl;
        private readonly Lazy<IGrpcLocationService> _grpcLocationService;
        private readonly Lazy<IGrpcAuditLogsClientImpl> _grpcAuditLogsClientImpl;

        public ServiceManager(AuthenticationService.AuthenticationServiceClient authService,
                              AppUserService.AppUserServiceClient userService,
                              LocationService.LocationServiceClient serviceLocation,
                              AuditLogsService.AuditLogsServiceClient logsServiceClient) 
        {
            _grpcAuthenticationServiceImpl = new Lazy<IGrpcAuthenticationServiceImpl>(()
                => new GrpcAuthenticationServiceImpl(authService));
            _grpcUserServiceImpl = new Lazy<IGrpcUserServiceImpl>(() 
                => new GrpcUserServiceImpl(userService));
            _grpcLocationService = new Lazy<IGrpcLocationService>(()
                => new GrpcLocationServiceImpl(serviceLocation));
            _grpcAuditLogsClientImpl = new Lazy<IGrpcAuditLogsClientImpl>(()
                => new GrpcAuditLogsClientImpl(logsServiceClient));
        }

        public IGrpcAuthenticationServiceImpl Authentication => _grpcAuthenticationServiceImpl.Value;
        public IGrpcUserServiceImpl User => _grpcUserServiceImpl.Value;
        public IGrpcLocationService Location => _grpcLocationService.Value;
        public IGrpcAuditLogsClientImpl AuditLogs => _grpcAuditLogsClientImpl.Value;
    }
}
