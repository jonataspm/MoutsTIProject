using Ambev.DeveloperEvaluation.Application.Carts.Dtos;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.CreateCart
{
    public record CreateCartRequest
    {
        public Guid UserId { get; init; }
        public DateTime Date { get; init; }
        public List<CartProductDto> Products { get; init; } = [];
    }
}
