using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Ambev.DeveloperEvaluation.UnitTests.Fixtures;
using MongoDB.Driver;
using Xunit;

namespace Ambev.DeveloperEvaluation.UnitTests.Integration.Repositories;

public class CartRepositoryMongoTests : IClassFixture<MongoDbFixture>, IAsyncLifetime
{
    private readonly CartRepositoryMongo _repository;
    private readonly MongoDbFixture _fixture;
    private const string CollectionName = "Carts";

    public CartRepositoryMongoTests(MongoDbFixture fixture)
    {
        _fixture = fixture;
        _repository = new CartRepositoryMongo(_fixture.Database);
    }

    public async Task InitializeAsync()
    {
        await _fixture.ClearCollectionAsync(CollectionName);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact(DisplayName = "Deve criar um carrinho no MongoDB e retornar a entidade")]
    public async Task CreateAsync_ShouldPersistInMongo()
    {
        // Arrange
        var cart = new Cart { UserId = Guid.NewGuid(), Date = DateTime.UtcNow };
        cart.Products.Add(new CartItem { ProductId = Guid.NewGuid(), Quantity = 2 });

        // Act
        var createdCart = await _repository.CreateAsync(cart);

        // Assert
        var collection = _fixture.Database.GetCollection<Cart>(CollectionName);
        var dbCart = await collection.Find(c => c.Id == createdCart.Id).FirstOrDefaultAsync();

        Assert.NotNull(dbCart);
        Assert.Equal(cart.UserId, dbCart.UserId);
        Assert.Single(dbCart.Products);
    }

    [Fact(DisplayName = "Deve buscar um carrinho pelo ID")]
    public async Task GetByIdAsync_ShouldReturnCart()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var cart = new Cart { Id = cartId, UserId = Guid.NewGuid() };
        var collection = _fixture.Database.GetCollection<Cart>(CollectionName);
        await collection.InsertOneAsync(cart);

        // Act
        var retrievedCart = await _repository.GetByIdAsync(cartId);

        // Assert
        Assert.NotNull(retrievedCart);
        Assert.Equal(cartId, retrievedCart.Id);
    }

    [Fact(DisplayName = "Deve substituir o documento inteiro ao atualizar (Update/Replace)")]
    public async Task UpdateAsync_ShouldReplaceEntireDocument()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var cart = new Cart { Id = cartId, UserId = Guid.NewGuid() };
        var collection = _fixture.Database.GetCollection<Cart>(CollectionName);
        await collection.InsertOneAsync(cart);

        // Act - Modificamos a entidade
        cart.Products.Add(new CartItem { ProductId = Guid.NewGuid(), Quantity = 5 });
        var updatedCart = await _repository.UpdateAsync(cart);

        // Assert
        var dbCart = await collection.Find(c => c.Id == cartId).FirstOrDefaultAsync();
        Assert.NotNull(dbCart);
        Assert.Single(dbCart.Products); // Garante que o array de produtos foi salvo embutido no documento
        Assert.Equal(5, dbCart.Products.First().Quantity);
    }

    [Fact(DisplayName = "Deve deletar o carrinho com sucesso")]
    public async Task DeleteAsync_ShouldRemoveDocument()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var cart = new Cart { Id = cartId, UserId = Guid.NewGuid() };
        var collection = _fixture.Database.GetCollection<Cart>(CollectionName);
        await collection.InsertOneAsync(cart);

        // Act
        var result = await _repository.DeleteAsync(cartId);

        // Assert
        Assert.True(result);
        var dbCart = await collection.Find(c => c.Id == cartId).FirstOrDefaultAsync();
        Assert.Null(dbCart);
    }

    [Fact(DisplayName = "Deve retornar lista paginada e ordenada")]
    public async Task GetPagedAsync_ShouldReturnPaginatedAndSorted()
    {
        // Arrange
        var collection = _fixture.Database.GetCollection<Cart>(CollectionName);

        // Criamos 3 carrinhos com datas diferentes
        var dateBase = DateTime.UtcNow;
        var carts = new List<Cart>
        {
            new() { Id = Guid.NewGuid(), Date = dateBase.AddDays(1) }, // Mais recente
            new() { Id = Guid.NewGuid(), Date = dateBase.AddDays(-1) }, // Mais antigo
            new() { Id = Guid.NewGuid(), Date = dateBase } // Meio
        };
        await collection.InsertManyAsync(carts);

        // Act - Busca página 1, tamanho 2, ordenado por Date DESC
        var (resultCarts, totalCarts) = await _repository.GetPagedAsync(1, 2, "date desc", CancellationToken.None);

        // Assert
        Assert.Equal(3, totalCarts);
        Assert.Equal(2, resultCarts.Count);

        // O primeiro deve ser o mais recente (AddDays(1))
        Assert.Equal(carts[0].Id, resultCarts[0].Id);

        // O segundo deve ser o do meio (dateBase)
        Assert.Equal(carts[2].Id, resultCarts[1].Id);
    }
}