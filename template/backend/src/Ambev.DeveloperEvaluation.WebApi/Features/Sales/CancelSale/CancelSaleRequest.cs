namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSale;

public record CancelSaleRequest
{
    public Guid Id { get; init; }
}