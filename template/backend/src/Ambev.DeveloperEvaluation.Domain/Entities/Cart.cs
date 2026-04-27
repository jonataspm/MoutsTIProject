using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Cart : BaseEntityMongo
{
    public Guid UserId { get; set; }
    public DateTime Date { get; set; }
    public List<CartItem> Products { get; set; } = [];
}

public class CartItem
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}