using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.UpdateUser;

public class UpdateUserProfile : Profile
{
    public UpdateUserProfile()
    {
        CreateMap<UpdateUserRequest, Application.Users.UpdateUser.UpdateUserCommand>();
        CreateMap<Application.Users.UpdateUser.UpdateUserResult, UpdateUserResponse>();
    }
}