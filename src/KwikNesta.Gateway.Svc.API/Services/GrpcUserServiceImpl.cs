using API.Common.Response.Model.Responses;
using Grpc.Core;
using KwikNesta.Gateway.Svc.API.DTOs;
using KwikNesta.Gateway.Svc.API.Grpc.Identity;
using KwikNesta.Gateway.Svc.API.Services.Interfaces;

namespace KwikNesta.Gateway.Svc.API.Services
{
    public class GrpcUserServiceImpl : IGrpcUserServiceImpl
    {
        private readonly AppUserService.AppUserServiceClient _serviceClient;

        public GrpcUserServiceImpl(AppUserService.AppUserServiceClient serviceClient)
        {
            _serviceClient = serviceClient;
        }

        public async Task<User> GetAuthenticatedUser(Metadata meta)
        {
            var loggedInUser = await _serviceClient
                .GetLoggedInUserAsync(new Empty { }, meta);

            return loggedInUser.User;
        }

        public async Task<GetPagedUsersResponse> GetPagedUsers(UserQuery query, Metadata meta)
        {
            var grpcQuery = new GetPagedUsersRequest
            {
                Page = query.Page,
                Size = query.PageSize,
                Search = query.Search
            };

            if (query.Status.HasValue)
            {
                grpcQuery.Status = query.Status.Value;
            }
            if (query.Gender.HasValue)
            {
                grpcQuery.Gender = query.Gender.Value;
            }

            var pagedUsersResponse = await _serviceClient.GetPagedUsersAsync(grpcQuery, meta);
            return pagedUsersResponse;
        }

        public async Task<User> GetUserById(string id, Metadata meta)
        {
            
            var userResponse = await _serviceClient.GetUserByIdAsync(new GetUserByIdRequest
            {
                UserId = id
            }, meta);

            return userResponse.User;
        }

        public async Task<UserStringResponse> UpdateBasicDetails(UpdateBasicUserDetailsRequest request, Metadata entries)
        {
            var updateResponse = await _serviceClient.UpdateBasicUserDetailsAsync(request, entries);
            return updateResponse.Response;
        }
    }
}
