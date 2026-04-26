using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Application.Products.Dtos;

namespace Ambev.DeveloperEvaluation.Application.Products.CreateProduct;

public class CreateProductProfile : Profile
{
    public CreateProductProfile()
    {
        CreateMap<RatingDto, Rating>().ReverseMap();

        CreateMap<CreateProductCommand, Product>();
        CreateMap<Product, CreateProductResult>();
    }
}