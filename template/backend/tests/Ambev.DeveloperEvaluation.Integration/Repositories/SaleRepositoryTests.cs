using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Ambev.DeveloperEvaluation.UnitTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ambev.DeveloperEvaluation.UnitTests.Integration.Repositories;

public class SaleRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly SaleRepository _repository;
    private readonly DatabaseFixture _fixture;

    public SaleRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _repository = new SaleRepository(_fixture.Context);
        _fixture.ClearDatabase();
    }

    [Fact(DisplayName = "Deve persistir uma venda com itens e recuperar com sucesso")]
    public async Task CreateAsync_ShouldPersistSaleWithItems()
    {
        // Arrange
        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = "202604270001",
            CustomerId = Guid.NewGuid(),
            CustomerName = "Integration Test",
            BranchId = Guid.NewGuid(),
            BranchName = "Main Branch",
            Date = DateTime.UtcNow
        };

        sale.Items.Add(new SaleItem(Guid.NewGuid(), "Product A", 5, 100m));
        sale.CalculateTotal();

        // Act
        var createdSale = await _repository.CreateAsync(sale, CancellationToken.None);

        // Assert
        var dbSale = await _fixture.Context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == createdSale.Id);

        Assert.NotNull(dbSale);
        Assert.Equal(sale.SaleNumber, dbSale.SaleNumber);
        Assert.Equal(450m, dbSale.TotalAmount);
        Assert.Single(dbSale.Items);
    }

    [Fact(DisplayName = "Deve gerar números de venda sequenciais usando a tabela de contador")]
    public async Task GenerateNextSaleNumberAsync_ShouldHandleAtomicIncrements()
    {
        // Act 
        var number1 = await _repository.GenerateNextSaleNumberAsync(CancellationToken.None);
        var number2 = await _repository.GenerateNextSaleNumberAsync(CancellationToken.None);
        var number3 = await _repository.GenerateNextSaleNumberAsync(CancellationToken.None);

        // Assert
        var todayPrefix = DateTime.UtcNow.ToString("yyyyMMdd");
        Assert.Equal($"{todayPrefix}0001", number1);
        Assert.Equal($"{todayPrefix}0002", number2);
        Assert.Equal($"{todayPrefix}0003", number3);

        var counter = await _fixture.Context.SaleCounters.FirstAsync();
        Assert.Equal(3, counter.LastNumber);
    }

    [Fact(DisplayName = "Deve remover itens órfãos ao atualizar a lista de itens da venda")]
    public async Task UpdateAsync_ShouldHandleOrphanRemoval()
    {
        // Arrange
        var sale = new Sale { Id = Guid.NewGuid(), SaleNumber = "UPDATE_TEST", CustomerName = "User" };
        var item1Id = Guid.NewGuid();
        sale.Items.Add(new SaleItem(item1Id, "Prod 1", 1, 10m));
        sale.Items.Add(new SaleItem(Guid.NewGuid(), "Prod 2", 1, 10m));

        await _fixture.Context.Sales.AddAsync(sale);
        await _fixture.Context.SaveChangesAsync();

        // Act
        var existingSale = await _repository.GetByIdAsync(sale.Id, CancellationToken.None);
        var itemToRemove = existingSale.Items.First(i => i.ProductId != item1Id);
        existingSale.Items.Remove(itemToRemove);
        existingSale.Items.Add(new SaleItem(Guid.NewGuid(), "Prod 3", 1, 20m));

        await _repository.UpdateAsync(existingSale, CancellationToken.None);

        // Assert
        var result = await _fixture.Context.Sales.Include(s => s.Items).FirstAsync(s => s.Id == sale.Id);
        Assert.Equal(2, result.Items.Count);
        Assert.DoesNotContain(result.Items, i => i.ProductId == itemToRemove.ProductId);
    }
}