using Ambev.DeveloperEvaluation.Application.Carts.Dtos;

namespace Ambev.DeveloperEvaluation.Application.Carts.CreateCart;

public record CreateCartResult
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public DateTime Date { get; init; }
    public List<CartProductDto> Products { get; init; } = [];
}