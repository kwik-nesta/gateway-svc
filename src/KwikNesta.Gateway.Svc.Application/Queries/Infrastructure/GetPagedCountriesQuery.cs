using KwikNesta.Contracts.Models;

namespace KwikNesta.Gateway.Svc.Application.Queries.Infrastructure
{
    public class GetPagedCountriesQuery : PageQuery
    {
        /// <summary>
        /// Search by country name, nationality, region or sub-region
        /// </summary>
        public string? Search { get; set; }
    }
}
