using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class SaleItem : BaseEntity
{
    public Guid SaleId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Discount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public bool IsCancelled { get; private set; }

    public SaleItem() { }

    public SaleItem(Guid productId, string productName, int quantity, decimal unitPrice)
    {
        if (quantity > 20)
            throw new InvalidOperationException("Não é possível vender acima de 20 itens idênticos.");
        if (quantity < 1)
            throw new InvalidOperationException("A quantidade deve ser maior que zero.");

        Id = Guid.NewGuid();
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;

        CalculateItemTotals();
    }

    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity > 20)
            throw new InvalidOperationException("Não é possível vender acima de 20 itens idênticos.");
        if (newQuantity < 1)
            throw new InvalidOperationException("A quantidade deve ser maior que zero.");

        Quantity = newQuantity;
        CalculateItemTotals();
    }

    public void Cancel()
    {
        IsCancelled = true;
        TotalAmount = 0;
        Discount = 0;
    }

    private void CalculateItemTotals()
    {
        var rawTotal = Quantity * UnitPrice;

        if (Quantity >= 10 && Quantity <= 20)
            Discount = rawTotal * 0.20m;
        else if (Quantity >= 4 && Quantity <= 9)
            Discount = rawTotal * 0.10m;
        else
            Discount = 0m;

        TotalAmount = rawTotal - Discount;
    }
}