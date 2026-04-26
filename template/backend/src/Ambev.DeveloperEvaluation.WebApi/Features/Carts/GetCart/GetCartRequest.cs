namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.GetCart;

public record GetCartRequest
{
    public Guid Id { get; init; }
}