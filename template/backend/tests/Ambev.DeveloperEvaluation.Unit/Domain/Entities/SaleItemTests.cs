using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.UnitTests.Domain.Entities;

public class SaleItemTests
{
    [Theory(DisplayName = "Should apply no discount for quantities strictly below 4")]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Constructor_QuantityBelow4_ShouldApplyNoDiscount(int quantity)
    {
        // Arrange
        var unitPrice = 10m;
        var expectedTotal = quantity * unitPrice;

        // Act
        var item = new SaleItem(Guid.NewGuid(), "Test Product", quantity, unitPrice);

        // Assert
        Assert.Equal(0m, item.Discount);
        Assert.Equal(expectedTotal, item.TotalAmount);
    }

    [Theory(DisplayName = "Should apply 10% discount for quantities between 4 and 9")]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(9)]
    public void Constructor_QuantityBetween4And9_ShouldApply10PercentDiscount(int quantity)
    {
        // Arrange
        var unitPrice = 10m;
        var rawTotal = quantity * unitPrice;
        var expectedDiscount = rawTotal * 0.10m;
        var expectedTotal = rawTotal - expectedDiscount;

        // Act
        var item = new SaleItem(Guid.NewGuid(), "Test Product", quantity, unitPrice);

        // Assert
        Assert.Equal(expectedDiscount, item.Discount);
        Assert.Equal(expectedTotal, item.TotalAmount);
    }

    [Theory(DisplayName = "Should apply 20% discount for quantities between 10 and 20")]
    [InlineData(10)]
    [InlineData(15)]
    [InlineData(20)]
    public void Constructor_QuantityBetween10And20_ShouldApply20PercentDiscount(int quantity)
    {
        // Arrange
        var unitPrice = 10m;
        var rawTotal = quantity * unitPrice;
        var expectedDiscount = rawTotal * 0.20m;
        var expectedTotal = rawTotal - expectedDiscount;

        // Act
        var item = new SaleItem(Guid.NewGuid(), "Test Product", quantity, unitPrice);

        // Assert
        Assert.Equal(expectedDiscount, item.Discount);
        Assert.Equal(expectedTotal, item.TotalAmount);
    }

    [Fact(DisplayName = "Should throw exception when quantity is greater than 20")]
    public void Constructor_QuantityAbove20_ShouldThrowException()
    {
        // Arrange
        var invalidQuantity = 21;

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            new SaleItem(Guid.NewGuid(), "Test Product", invalidQuantity, 10m));

        Assert.Equal("Não é possível vender acima de 20 itens idênticos.", exception.Message);
    }

    [Fact(DisplayName = "Should calculate correct totals when cancelling an item")]
    public void Cancel_WhenCalled_ShouldZeroOutTotals()
    {
        // Arrange
        var item = new SaleItem(Guid.NewGuid(), "Test Product", 5, 10m);

        // Act
        item.Cancel();

        // Assert
        Assert.True(item.IsCancelled);
        Assert.Equal(0m, item.TotalAmount);
        Assert.Equal(0m, item.Discount);
    }
}