using Ambev.DeveloperEvaluation.Application.Sales.Dtos;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

public record CreateSaleResponse
{
    public Guid Id { get; init; }
    public string SaleNumber { get; init; } = string.Empty;
    public DateTime Date { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public string BranchName { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public bool IsCancelled { get; init; }
    public List<SaleItemResultDto> Items { get; init; } = [];
}