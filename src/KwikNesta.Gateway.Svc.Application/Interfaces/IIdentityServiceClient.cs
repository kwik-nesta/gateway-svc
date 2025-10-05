using KwikNesta.Contracts.Models;
using KwikNesta.Gateway.Svc.Application.Commands.Identity;
using KwikNesta.Gateway.Svc.Application.DTOs.Identity;
using Refit;

namespace KwikNesta.Gateway.Svc.Application.Interfaces
{
    public interface IIdentityServiceClient
    {
        private const string ApiBaseV1 = "/api/v1/auth";

        [Post($"{ApiBaseV1}/sign-in")]
        Task<ApiResponse<ApiResult<LoginResponseDto>>> LoginAsyncV1([Body] LoginCommand command);
        [Post($"{ApiBaseV1}/sign-up")]
        Task<ApiResponse<ApiResult<RegisterResponseDto>>> RegisterAsyncV1([Body] RegisterCommand command);
        [Put($"{ApiBaseV1}/confirm")]
        Task<ApiResponse<ApiResult<string>>> VerifyAccount([Body] VerificationCommand command);
    }
}
