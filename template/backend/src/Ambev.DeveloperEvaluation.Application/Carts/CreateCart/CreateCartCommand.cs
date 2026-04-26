using MediatR;
using Ambev.DeveloperEvaluation.Application.Carts.Dtos;

namespace Ambev.DeveloperEvaluation.Application.Carts.CreateCart;

public record CreateCartCommand : IRequest<CreateCartResult>
{
    public Guid UserId { get; init; }
    public DateTime Date { get; init; }
    public List<CartProductDto> Products { get; init; } = [];
}