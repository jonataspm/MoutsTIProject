namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.ListProductsByCategory;

public record ListProductsByCategoryRequest
{
    public string Category { get; init; } = string.Empty;
    public int? Page { get; init; } = 1;
    public int? Size { get; init; } = 10;
    public string? Order { get; init; }
}