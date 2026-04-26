using MediatR;
using Ambev.DeveloperEvaluation.Application.Sales.Dtos;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public record CreateSaleCommand : IRequest<CreateSaleResult>
{
    public string SaleNumber { get; init; } = string.Empty;
    public DateTime Date { get; init; }
    public Guid CustomerId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public Guid BranchId { get; init; }
    public string BranchName { get; init; } = string.Empty;
    public List<SaleItemDto> Items { get; init; } = [];
}