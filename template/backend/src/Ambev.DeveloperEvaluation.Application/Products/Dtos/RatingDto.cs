namespace Ambev.DeveloperEvaluation.Application.Products.Dtos;

public record RatingDto
{
    public decimal Rate { get; init; }
    public int Count { get; init; }
}