using Ambev.DeveloperEvaluation.Application.Carts.CreateCart;
using Ambev.DeveloperEvaluation.Application.Carts.Dtos;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using Bogus;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.UnitTests.Application.Carts;

public class CreateCartHandlerTests
{
    private readonly ICartRepository _repositoryMock;
    private readonly IMapper _mapperMock;
    private readonly CreateCartHandler _handler;

    public CreateCartHandlerTests()
    {
        _repositoryMock = Substitute.For<ICartRepository>();
        _mapperMock = Substitute.For<IMapper>();
        _handler = new CreateCartHandler(_repositoryMock, _mapperMock);
    }

    [Fact(DisplayName = "Deve criar um carrinho com sucesso para um utilizador válido")]
    public async Task Handle_ValidCommand_ShouldCreateCartAndReturnResult()
    {
        // Arrange
        var cartItemFaker = new Faker<CartProductDto>()
            .RuleFor(i => i.ProductId, f => f.Random.Guid())
            .RuleFor(i => i.Quantity, f => f.Random.Int(1, 10));

        var command = new Faker<CreateCartCommand>()
            .RuleFor(c => c.UserId, f => f.Random.Guid())
            .RuleFor(c => c.Date, f => f.Date.Recent())
            .RuleFor(c => c.Products, f => cartItemFaker.Generate(f.Random.Int(1, 5)))
            .Generate();

        var cartEntity = new Cart { Id = Guid.NewGuid(), UserId = command.UserId };
        var expectedResult = new CreateCartResult { Id = cartEntity.Id };

        _mapperMock.Map<Cart>(command).Returns(cartEntity);
        _repositoryMock.CreateAsync(cartEntity, Arg.Any<CancellationToken>()).Returns(cartEntity);
        _mapperMock.Map<CreateCartResult>(cartEntity).Returns(expectedResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResult.Id, result.Id);
        await _repositoryMock.Received(1).CreateAsync(Arg.Any<Cart>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Deve lançar ValidationException quando o comando for inválido")]
    public async Task Handle_InvalidCommand_ShouldThrowValidationException()
    {
        // Arrange
        var command = new CreateCartCommand { UserId = Guid.Empty };

        // Act & Assert
        await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => _handler.Handle(command, CancellationToken.None));
        await _repositoryMock.DidNotReceive().CreateAsync(Arg.Any<Cart>(), Arg.Any<CancellationToken>());
    }
}