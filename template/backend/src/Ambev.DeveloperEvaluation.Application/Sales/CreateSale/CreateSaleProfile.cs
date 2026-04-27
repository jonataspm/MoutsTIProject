using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Application.Sales.Dtos;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleProfile : Profile
{
    public CreateSaleProfile()
    {
        CreateMap<SaleItemDto, SaleItem>()
            .ConstructUsing(dto => new SaleItem(dto.ProductId, dto.ProductName, dto.Quantity, dto.UnitPrice));

        CreateMap<CreateSaleCommand, Sale>();

        CreateMap<SaleItem, SaleItemResultDto>();
        CreateMap<Sale, CreateSaleResult>();
    }
}