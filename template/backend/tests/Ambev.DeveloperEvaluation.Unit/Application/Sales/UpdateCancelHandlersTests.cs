using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class UpdateCancelHandlersTests
{
    private readonly ISaleRepository _saleRepository = Substitute.For<ISaleRepository>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly ILogger<UpdateSaleHandler> _updateLogger = Substitute.For<ILogger<UpdateSaleHandler>>();
    private readonly ILogger<CancelSaleHandler> _cancelLogger = Substitute.For<ILogger<CancelSaleHandler>>();
    private readonly ILogger<CancelSaleItemHandler> _cancelItemLogger = Substitute.For<ILogger<CancelSaleItemHandler>>();

    [Fact]
    public async Task UpdateSaleHandler_WhenSaleExists_MergesItemsAndCallsUpdate()
    {
        // Arrange
        var existing = new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = "202601010001",
            Items = new List<SaleItem>
            {
                new SaleItem(Guid.NewGuid(), "A", 2, 10m)
            }
        };

        var command = new UpdateSaleCommand
        {
            Id = existing.Id,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Client",
            BranchId = Guid.NewGuid(),
            BranchName = "Branch",
            Date = DateTime.UtcNow,
            Items = new List<Ambev.DeveloperEvaluation.Application.Sales.Dtos.SaleItemDto>
            {
                new Ambev.DeveloperEvaluation.Application.Sales.Dtos.SaleItemDto { ProductId = existing.Items[0].ProductId, ProductName = "A", Quantity = 5, UnitPrice = 10m },
                new Ambev.DeveloperEvaluation.Application.Sales.Dtos.SaleItemDto { ProductId = Guid.NewGuid(), ProductName = "B", Quantity = 1, UnitPrice = 5m }
            }
        };

        _saleRepository.GetByIdAsync(existing.Id, Arg.Any<CancellationToken>()).Returns(Task.FromResult(existing));
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(existing));
        var handler = new UpdateSaleHandler(_saleRepository, _mapper, _updateLogger);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        await _saleRepository.Received(1).GetByIdAsync(existing.Id, Arg.Any<CancellationToken>());
        await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CancelSaleHandler_WhenSaleExists_CancelsAndUpdates()
    {
        // Arrange
        var sale = new Sale { Id = Guid.NewGuid(), SaleNumber = "202601010002", Items = new List<SaleItem> { new SaleItem(Guid.NewGuid(), "X", 1, 1m) } };
        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(Task.FromResult(sale));
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(sale));

        var handler = new CancelSaleHandler(_saleRepository, _cancelLogger);
        var command = new CancelSaleCommand(sale.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CancelSaleItemHandler_WhenSaleAndItemExists_CancelsItemAndUpdates()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = "202601010003",
            Items = new List<SaleItem>
            {
                new SaleItem(Guid.NewGuid(), "X", 2, 5m) { Id = itemId }
            }
        };

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(Task.FromResult(sale));
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(sale));

        var handler = new CancelSaleItemHandler(_saleRepository, _cancelItemLogger);
        var command = new CancelSaleItemCommand(sale.Id, itemId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }
}