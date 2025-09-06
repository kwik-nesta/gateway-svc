namespace KwikNesta.Gateway.Svc.API.Services.Interfaces
{
    public interface IServiceManager
    {
        IGrpcAuthenticationServiceImpl Authentication { get; }
        IGrpcUserServiceImpl User { get; }
    }
}
