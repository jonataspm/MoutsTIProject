namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ListSales;

public record ListSalesRequest
{
    public int? Page { get; init; } = 1;
    public int? Size { get; init; } = 10;
    public string? Order { get; init; }
}