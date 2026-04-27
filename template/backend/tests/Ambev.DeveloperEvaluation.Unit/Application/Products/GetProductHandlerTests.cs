using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.UnitTests.Application.Products;

public class GetProductHandlerTests
{
    private readonly IProductRepository _repositoryMock;
    private readonly IMapper _mapperMock;
    private readonly GetProductHandler _handler;

    public GetProductHandlerTests()
    {
        _repositoryMock = Substitute.For<IProductRepository>();
        _mapperMock = Substitute.For<IMapper>();
        _handler = new GetProductHandler(_repositoryMock, _mapperMock);
    }

    [Fact(DisplayName = "Deve retornar o produto quando o ID existe")]
    public async Task Handle_ProductExists_ShouldReturnProduct()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product { Id = productId, Title = "Smartphone" };
        var expectedResult = new GetProductResult { Id = productId, Title = "Smartphone" };

        _repositoryMock.GetByIdAsync(productId, Arg.Any<CancellationToken>()).Returns(product);
        _mapperMock.Map<GetProductResult>(product).Returns(expectedResult);

        // Act
        var result = await _handler.Handle(new GetProductCommand(productId), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Smartphone", result.Title);
    }

    [Fact(DisplayName = "Deve lançar KeyNotFoundException quando o produto não existe")]
    public async Task Handle_ProductNotFound_ShouldThrowException()
    {
        // Arrange
        _repositoryMock.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Product)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(new GetProductCommand(Guid.NewGuid()), CancellationToken.None));
    }
}