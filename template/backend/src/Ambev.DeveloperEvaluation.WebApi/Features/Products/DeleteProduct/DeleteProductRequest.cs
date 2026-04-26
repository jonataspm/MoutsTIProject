namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.DeleteProduct;

public record DeleteProductRequest
{
    public Guid Id { get; init; }
}