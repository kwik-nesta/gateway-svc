using KwikNesta.Contracts.Enums;
using KwikNesta.Contracts.Models;

namespace KwikNesta.Gateway.Svc.Application.Queries.Identity
{
    public class GetPagedUsersQuery : PageQuery
    {
        public string Search { get; set; } = string.Empty;
        public Gender? Gender { get; set; }
        public UserStatus? Status { get; set; }
    }
}
