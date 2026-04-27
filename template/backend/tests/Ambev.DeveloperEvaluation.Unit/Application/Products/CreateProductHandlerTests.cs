using Ambev.DeveloperEvaluation.Application.Products.CreateProduct;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using Bogus;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.UnitTests.Application.Products;

public class CreateProductHandlerTests
{
    private readonly IProductRepository _repositoryMock;
    private readonly IMapper _mapperMock;
    private readonly CreateProductHandler _handler;

    public CreateProductHandlerTests()
    {
        _repositoryMock = Substitute.For<IProductRepository>();
        _mapperMock = Substitute.For<IMapper>();
        _handler = new CreateProductHandler(_repositoryMock, _mapperMock);
    }

    [Fact(DisplayName = "Deve criar um produto com sucesso quando os dados são válidos")]
    public async Task Handle_ValidCommand_ShouldCreateProductAndReturnResult()
    {
        // Arrange
        var command = new Faker<CreateProductCommand>()
            .RuleFor(p => p.Title, f => f.Commerce.ProductName())
            .RuleFor(p => p.Price, f => f.Random.Decimal(1, 1000))
            .RuleFor(p => p.Category, f => f.Commerce.Categories(1)[0])
            .Generate();

        var productEntity = new Product { Id = Guid.NewGuid(), Title = command.Title };
        var expectedResult = new CreateProductResult { Id = productEntity.Id };

        _mapperMock.Map<Product>(command).Returns(productEntity);
        _repositoryMock.CreateAsync(productEntity, Arg.Any<CancellationToken>()).Returns(productEntity);
        _mapperMock.Map<CreateProductResult>(productEntity).Returns(expectedResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResult.Id, result.Id);
        await _repositoryMock.Received(1).CreateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }
}