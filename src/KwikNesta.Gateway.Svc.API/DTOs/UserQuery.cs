using KwikNesta.Contracts.Models;
using KwikNesta.Gateway.Svc.API.Grpc.Identity;

namespace KwikNesta.Gateway.Svc.API.DTOs
{
    public class UserQuery : PageQuery
    {
        public string Search { get; set; } = string.Empty;
        public GrpcUserGender? Gender { get; set; }
        public GrpcUserStatus? Status { get; set; }
    }
}
