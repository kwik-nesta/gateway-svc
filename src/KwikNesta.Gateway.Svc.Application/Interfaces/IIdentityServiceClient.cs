using KwikNesta.Contracts.Models;
using KwikNesta.Gateway.Svc.Application.Commands.Identity;
using KwikNesta.Gateway.Svc.Application.DTOs.Identity;
using Refit;

namespace KwikNesta.Gateway.Svc.Application.Interfaces
{
    public interface IIdentityServiceClient
    {
        private const string ApiBaseV1 = "/api/v1";

        [Post($"{ApiBaseV1}/auth/sign-in")]
        Task<ApiResponse<ApiResult<LoginResponseDto>>> LoginAsyncV1([Body] LoginCommand command);
    }
}
