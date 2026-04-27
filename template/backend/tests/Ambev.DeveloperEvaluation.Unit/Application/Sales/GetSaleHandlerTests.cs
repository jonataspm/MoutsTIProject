using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.UnitTests.Application.Sales;

public class GetSaleHandlerTests
{
    private readonly ISaleRepository _repositoryMock;
    private readonly IMapper _mapperMock;
    private readonly GetSaleHandler _handler;

    public GetSaleHandlerTests()
    {
        _repositoryMock = Substitute.For<ISaleRepository>();
        _mapperMock = Substitute.For<IMapper>();
        _handler = new GetSaleHandler(_repositoryMock, _mapperMock);
    }

    [Fact(DisplayName = "Deve retornar o resultado mapeado quando a venda existe")]
    public async Task Handle_ExistingSale_ShouldReturnMappedResult()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var sale = new Sale { Id = saleId, SaleNumber = "TEST123" };
        var expectedResult = new GetSaleResult { Id = saleId, SaleNumber = "TEST123" };

        _repositoryMock.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(sale);
        _mapperMock.Map<GetSaleResult>(sale).Returns(expectedResult);

        // Act
        var result = await _handler.Handle(new GetSaleCommand(saleId), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(saleId, result.Id);
        _mapperMock.Received(1).Map<GetSaleResult>(sale);
    }
}