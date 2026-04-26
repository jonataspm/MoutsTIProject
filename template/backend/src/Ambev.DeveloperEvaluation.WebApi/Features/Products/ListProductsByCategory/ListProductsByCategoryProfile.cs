using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Products.ListProductsByCategory;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.ListProductsByCategory;

public class ListProductsByCategoryProfile : Profile
{
    public ListProductsByCategoryProfile()
    {
        CreateMap<ListProductsByCategoryRequest, ListProductsByCategoryCommand>();
        CreateMap<ListProductsByCategoryResult, ListProductsByCategoryResponse>();
    }
}