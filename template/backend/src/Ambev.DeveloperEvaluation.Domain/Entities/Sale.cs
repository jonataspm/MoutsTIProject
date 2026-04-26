using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Sale : BaseEntity
{
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;

    public bool IsCancelled { get; private set; }
    public decimal TotalAmount { get; private set; }

    public List<SaleItem> Items { get; set; } = new();

    public void CalculateTotal()
    {
        TotalAmount = Items.Where(i => !i.IsCancelled).Sum(i => i.TotalAmount);
    }

    public void Cancel()
    {
        IsCancelled = true;
        foreach (var item in Items)
        {
            item.Cancel();
        }
        TotalAmount = 0;
    }
}
