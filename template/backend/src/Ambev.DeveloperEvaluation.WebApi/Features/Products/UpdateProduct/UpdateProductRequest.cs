using Ambev.DeveloperEvaluation.Application.Products.Dtos;
using System.Text.Json.Serialization;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.UpdateProduct;

public record UpdateProductRequest
{
    [JsonIgnore]
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string Description { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string Image { get; init; } = string.Empty;
    public RatingDto Rating { get; init; } = new();
}