using Ambev.DeveloperEvaluation.Application.Carts.GetCart;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.UnitTests.Application.Carts;

public class GetCartHandlerTests
{
    private readonly ICartRepository _repositoryMock;
    private readonly IMapper _mapperMock;
    private readonly GetCartHandler _handler;

    public GetCartHandlerTests()
    {
        _repositoryMock = Substitute.For<ICartRepository>();
        _mapperMock = Substitute.For<IMapper>();
        _handler = new GetCartHandler(_repositoryMock, _mapperMock);
    }

    [Fact(DisplayName = "Deve retornar o carrinho mapeado quando ID for encontrado")]
    public async Task Handle_CartExists_ShouldReturnMappedResult()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var cart = new Cart { Id = cartId };
        var expectedResult = new GetCartResult { Id = cartId };

        _repositoryMock.GetByIdAsync(cartId, Arg.Any<CancellationToken>()).Returns(cart);
        _mapperMock.Map<GetCartResult>(cart).Returns(expectedResult);

        // Act
        var result = await _handler.Handle(new GetCartCommand(cartId), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(cartId, result.Id);
    }

    [Fact(DisplayName = "Deve lançar KeyNotFoundException quando o carrinho não existir")]
    public async Task Handle_CartNotFound_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        _repositoryMock.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Cart)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(new GetCartCommand(Guid.NewGuid()), CancellationToken.None));
    }
}