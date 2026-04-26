using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.UpdateCart;

public class UpdateCartProfile : Profile
{
    public UpdateCartProfile()
    {
        CreateMap<UpdateCartRequest, Application.Carts.UpdateCart.UpdateCartCommand>();
        CreateMap<Application.Carts.UpdateCart.UpdateCartResult, UpdateCartResponse>();
    }
}