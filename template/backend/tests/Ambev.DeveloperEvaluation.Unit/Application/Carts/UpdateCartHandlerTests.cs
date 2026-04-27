using Ambev.DeveloperEvaluation.Application.Carts.Dtos;
using Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.UnitTests.Application.Carts;

public class UpdateCartHandlerTests
{
    private readonly ICartRepository _repositoryMock;
    private readonly IMapper _mapperMock;
    private readonly UpdateCartHandler _handler;

    public UpdateCartHandlerTests()
    {
        _repositoryMock = Substitute.For<ICartRepository>();
        _mapperMock = Substitute.For<IMapper>();
        _handler = new UpdateCartHandler(_repositoryMock, _mapperMock);
    }

    [Fact(DisplayName = "Deve atualizar itens do carrinho e persistir as mudanças")]
    public async Task Handle_ExistingCart_ShouldUpdateItemsSuccessfully()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var existingCart = new Cart { Id = cartId, UserId = Guid.NewGuid() };
        var productId = Guid.NewGuid();

        var command = new UpdateCartCommand
        {
            Id = cartId,
            Products = new List<CartProductDto> { new CartProductDto { ProductId = productId, Quantity = 5 } }
        };

        var mappedItems = new List<CartItem> { new CartItem { ProductId = productId, Quantity = 5 } };

        _repositoryMock.GetByIdAsync(cartId, Arg.Any<CancellationToken>()).Returns(existingCart);
        _repositoryMock.UpdateAsync(Arg.Any<Cart>(), Arg.Any<CancellationToken>()).Returns(existingCart);

        _mapperMock.Map<List<CartItem>>(command.Products).Returns(mappedItems);

        _mapperMock.Map(command, existingCart).Returns(existingCart).AndDoes(x =>
        {
            existingCart.Products.Clear();
            foreach (var item in mappedItems) existingCart.Products.Add(item);
        });

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _repositoryMock.Received(1).UpdateAsync(Arg.Is<Cart>(cart =>
            cart.Products.Count == 1 &&
            cart.Products.First().Quantity == 5 &&
            cart.Products.First().ProductId == productId
        ), Arg.Any<CancellationToken>());
    }
}