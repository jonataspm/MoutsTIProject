using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Ambev.DeveloperEvaluation.UnitTests.Fixtures;
using Xunit;

namespace Ambev.DeveloperEvaluation.UnitTests.Integration.Repositories;

public class ProductRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly ProductRepository _repository;
    private readonly DatabaseFixture _fixture;

    public ProductRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _repository = new ProductRepository(_fixture.Context);
        _fixture.ClearDatabase();
    }

    [Fact(DisplayName = "Deve retornar produtos paginados corretamente")]
    public async Task GetPagedAsync_ShouldReturnCorrectPage()
    {
        // Arrange - Cria 15 produtos
        for (int i = 1; i <= 15; i++)
        {
            await _repository.CreateAsync(new Product { Title = $"Prod {i}", Category = "Test" }, default);
        }

        // Act - Pede página 2 com tamanho 10
        var (products, total) = await _repository.GetPagedAsync(2, 10, "title asc", default);

        // Assert
        Assert.Equal(15, total);
        Assert.Equal(5, products.Count()); // Página 2 deve ter os 5 restantes
    }

    [Fact(DisplayName = "Deve filtrar produtos por categoria")]
    public async Task GetByCategory_ShouldFilterCorrectly()
    {
        // Arrange
        await _repository.CreateAsync(new Product { Title = "TV", Category = "Electronics" }, default);
        await _repository.CreateAsync(new Product { Title = "Shirt", Category = "Clothing" }, default);

        // Act
        var (products, total) = await _repository.GetPagedByCategoryAsync("Electronics", 1, 10, "", default);

        // Assert
        Assert.Single(products);
        Assert.Equal("Electronics", products.First().Category);
    }
}