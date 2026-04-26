namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.ListCarts;

public record ListCartsRequest
{
    public int? Page { get; init; }
    public int? Size { get; init; }
    public string? Order { get; init; }
}