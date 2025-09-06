using KwikNesta.Contracts.Models;

namespace KwikNesta.Gateway.Svc.API.DTOs
{
    public class PagedDataQuery : PageQuery
    {
        public string Search { get; set; } = string.Empty;
    }
}
