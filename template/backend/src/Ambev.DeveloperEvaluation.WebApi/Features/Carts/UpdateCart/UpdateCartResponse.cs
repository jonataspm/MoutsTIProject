using Ambev.DeveloperEvaluation.Application.Carts.Dtos;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.UpdateCart;

public record UpdateCartResponse
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public DateTime Date { get; init; }
    public List<CartProductDto> Products { get; init; } = [];
}