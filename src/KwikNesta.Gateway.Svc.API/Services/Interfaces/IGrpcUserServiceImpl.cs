using Grpc.Core;
using KwikNesta.Gateway.Svc.API.DTOs;
using KwikNesta.Gateway.Svc.API.Grpc.Identity;

namespace KwikNesta.Gateway.Svc.API.Services.Interfaces
{
    public interface IGrpcUserServiceImpl
    {
        Task<User> GetAuthenticatedUser(Metadata meta);
        Task<GetPagedUsersResponse> GetPagedUsers(UserQuery query, Metadata meta);
        Task<User> GetUserById(string id, Metadata meta);
        Task<UserStringResponse> UpdateBasicDetails(UpdateBasicUserDetailsRequest request, Metadata entries);
    }
}
