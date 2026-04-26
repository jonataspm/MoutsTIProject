using Ambev.DeveloperEvaluation.Application.Carts.Dtos;
using System.Text.Json.Serialization;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.UpdateCart;

public record UpdateCartRequest
{
    [JsonIgnore]
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public DateTime Date { get; init; }
    public List<CartProductDto> Products { get; init; } = [];
}