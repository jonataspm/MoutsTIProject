using Ambev.DeveloperEvaluation.Application.Products.ListProducts;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Common.Data;
using AutoMapper;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.UnitTests.Application.Products;

public class ListProductsHandlerTests
{
    private readonly IProductRepository _repositoryMock;
    private readonly IMapper _mapperMock;
    private readonly ListProductsHandler _handler;

    public ListProductsHandlerTests()
    {
        _repositoryMock = Substitute.For<IProductRepository>();
        _mapperMock = Substitute.For<IMapper>();
        _handler = new ListProductsHandler(_repositoryMock, _mapperMock);
    }

    [Fact(DisplayName = "Deve retornar lista paginada de produtos")]
    public async Task Handle_ValidPagination_ShouldReturnPaginatedResults()
    {
        // Arrange
        var command = new ListProductsCommand { Page = 1, Size = 10, Order = "title asc" };
        var products = new List<Product> { new Product { Title = "Product 1" } };

        _repositoryMock.GetPagedAsync(command.Page, command.Size, command.Order, Arg.Any<CancellationToken>())
            .Returns((products, 1));

        _mapperMock.Map<List<ListProductsResult>>(products)
            .Returns(new List<ListProductsResult> { new ListProductsResult { Title = "Product 1" } });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Data);
        Assert.Equal(1, result.TotalItems);
        await _repositoryMock.Received(1).GetPagedAsync(1, 10, "title asc", Arg.Any<CancellationToken>());
    }
}