namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.GetProduct;

public record GetProductRequest
{
    public Guid Id { get; init; }
}