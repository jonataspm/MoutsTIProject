namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSaleItem;

public record CancelSaleItemRequest
{
    public Guid Id { get; init; }
    public Guid ItemId { get; init; }
}