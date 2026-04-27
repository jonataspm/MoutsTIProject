using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Ambev.DeveloperEvaluation.UnitTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ambev.DeveloperEvaluation.UnitTests.Integration.Repositories;

public class CartRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly CartRepository _repository;
    private readonly DatabaseFixture _fixture;

    public CartRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _repository = new CartRepository(_fixture.Context);
        _fixture.ClearDatabase();
    }

    [Fact(DisplayName = "Deve criar um carrinho com itens e persistir no banco de dados")]
    public async Task CreateAsync_ShouldPersistCartWithItems()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var cart = new Cart
        {
            Id = cartId,
            UserId = Guid.NewGuid(),
            Date = DateTime.UtcNow
        };
        cart.Products.Add(new CartItem { ProductId = Guid.NewGuid(), Quantity = 3 });

        // Act
        var createdCart = await _repository.CreateAsync(cart, CancellationToken.None);

        // Assert
        var dbCart = await _fixture.Context.Carts
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == createdCart.Id);

        Assert.NotNull(dbCart);
        Assert.Equal(cart.UserId, dbCart.UserId);
        Assert.Single(dbCart.Products);
        Assert.Equal(3, dbCart.Products.First().Quantity);
    }

    [Fact(DisplayName = "Deve buscar um carrinho pelo ID trazendo seus itens (Eager Loading)")]
    public async Task GetByIdAsync_ShouldReturnCartWithItems()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var cart = new Cart { Id = cartId, UserId = Guid.NewGuid(), Date = DateTime.UtcNow };
        cart.Products.Add(new CartItem { ProductId = Guid.NewGuid(), Quantity = 5 });

        await _fixture.Context.Carts.AddAsync(cart);
        await _fixture.Context.SaveChangesAsync();

        // Act
        var retrievedCart = await _repository.GetByIdAsync(cartId, CancellationToken.None);

        // Assert
        Assert.NotNull(retrievedCart);
        Assert.Equal(cartId, retrievedCart.Id);
        Assert.NotEmpty(retrievedCart.Products);
    }


    [Fact(DisplayName = "Deve deletar o carrinho com sucesso")]
    public async Task DeleteAsync_ShouldRemoveCartFromDatabase()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var cart = new Cart { Id = cartId, UserId = Guid.NewGuid(), Date = DateTime.UtcNow };

        await _fixture.Context.Carts.AddAsync(cart);
        await _fixture.Context.SaveChangesAsync();

        // Act
        var isDeleted = await _repository.DeleteAsync(cartId, CancellationToken.None);

        // Assert
        Assert.True(isDeleted);
        var dbCart = await _fixture.Context.Carts.FirstOrDefaultAsync(c => c.Id == cartId);
        Assert.Null(dbCart);
    }
}