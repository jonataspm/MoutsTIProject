using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Products.ListProducts;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.ListProducts;

public class ListProductsProfile : Profile
{
    public ListProductsProfile()
    {
        CreateMap<ListProductsRequest, ListProductsCommand>();
        CreateMap<ListProductsResult, ListProductsResponse>();
    }
}