namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.DeleteCart;

public record DeleteCartRequest
{
    public Guid Id { get; init; }
}