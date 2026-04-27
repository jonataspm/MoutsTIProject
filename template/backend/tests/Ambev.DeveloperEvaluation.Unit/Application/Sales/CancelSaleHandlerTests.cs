using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.UnitTests.Application.Sales;

public class CancelSaleHandlerTests
{
    private readonly ISaleRepository _repositoryMock;
    private readonly ILogger<CancelSaleHandler> _loggerMock;
    private readonly CancelSaleHandler _handler;

    public CancelSaleHandlerTests()
    {
        _repositoryMock = Substitute.For<ISaleRepository>();
        _loggerMock = Substitute.For<ILogger<CancelSaleHandler>>();
        _handler = new CancelSaleHandler(_repositoryMock, _loggerMock);
    }

    [Fact(DisplayName = "Deve cancelar uma venda ativa com sucesso")]
    public async Task Handle_ActiveSale_ShouldCancelSuccessfully()
    {
        // Arrange
        var sale = new Sale { Id = Guid.NewGuid() };
        var command = new CancelSaleCommand(sale.Id);

        _repositoryMock.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.True(sale.IsCancelled);
        await _repositoryMock.Received(1).UpdateAsync(sale, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Deve lançar InvalidOperationException ao cancelar venda já cancelada")]
    public async Task Handle_AlreadyCancelled_ShouldThrowException()
    {
        // Arrange
        var sale = new Sale { Id = Guid.NewGuid() };
        sale.Cancel();
        var command = new CancelSaleCommand(sale.Id);
        _repositoryMock.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
    }
}