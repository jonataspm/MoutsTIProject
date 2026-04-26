namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.ListProducts;

public record ListProductsRequest
{
    public int? Page { get; init; } = 1;
    public int? Size { get; init; } = 10;
    public string? Order { get; init; }
}