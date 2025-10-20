using KwikNesta.Contracts.Models;
using KwikNesta.Gateway.Svc.Application.DTOs;
using KwikNesta.Gateway.Svc.Application.Queries.Infrastructure;
using Refit;

namespace KwikNesta.Gateway.Svc.Application.Interfaces
{
    public interface IInfrastructureServiceClient
    {
        #region Audit Endpoints
        [Get("/api/v1/audit-trails")]
        Task<ApiResponse<ApiResult<Paginator<AuditTrailDto>>>> GetAuditsV1([Query] GetAuditsQuery query);
        #endregion

        #region Location Endpoints

        #endregion
    }
}