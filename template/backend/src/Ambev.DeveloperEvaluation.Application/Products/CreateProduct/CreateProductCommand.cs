using MediatR;
using Ambev.DeveloperEvaluation.Application.Products.Dtos;

namespace Ambev.DeveloperEvaluation.Application.Products.CreateProduct;

public record CreateProductCommand : IRequest<CreateProductResult>
{
    public string Title { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string Description { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string Image { get; init; } = string.Empty;
    public RatingDto Rating { get; init; } = new();
}