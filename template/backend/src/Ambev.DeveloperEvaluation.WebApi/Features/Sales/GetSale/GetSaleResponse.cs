using Ambev.DeveloperEvaluation.Application.Sales.Dtos;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;

public record GetSaleResponse
{
    public Guid Id { get; init; }
    public string SaleNumber { get; init; } = string.Empty;
    public DateTime Date { get; init; }
    public Guid CustomerId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public Guid BranchId { get; init; }
    public string BranchName { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public bool IsCancelled { get; init; }
    public List<SaleItemResultDto> Items { get; init; } = [];
}