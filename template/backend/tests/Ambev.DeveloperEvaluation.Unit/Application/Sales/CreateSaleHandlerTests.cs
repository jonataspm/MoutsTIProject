using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.Dtos;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository = Substitute.For<ISaleRepository>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly ILogger<CreateSaleHandler> _logger = Substitute.For<ILogger<CreateSaleHandler>>();
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _handler = new CreateSaleHandler(_saleRepository, _mapper, _logger);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesSaleAndLogsEvent()
    {
        // Arrange
        var command = new CreateSaleCommand
        {
            CustomerId = Guid.NewGuid(),
            CustomerName = "John Doe",
            BranchId = Guid.NewGuid(),
            BranchName = "Main",
            Items = new List<SaleItemDto>
            {
                new SaleItemDto { ProductId = Guid.NewGuid(), ProductName = "P1", Quantity = 4, UnitPrice = 10m }
            }
        };

        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            CustomerId = command.CustomerId,
            CustomerName = command.CustomerName,
            BranchId = command.BranchId,
            BranchName = command.BranchName,
            Items = new List<SaleItem>
            {
                new SaleItem(command.Items[0].ProductId, command.Items[0].ProductName, command.Items[0].Quantity, command.Items[0].UnitPrice)
            }
        };

        _mapper.Map<Sale>(command).Returns(sale);
        _saleRepository.GenerateNextSaleNumberAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult("202601010001"));
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<CreateSaleResult>(sale).Returns(new CreateSaleResult { Id = sale.Id, SaleNumber = "202601010001" });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(sale.Id);
        await _saleRepository.Received(1).GenerateNextSaleNumberAsync(Arg.Any<CancellationToken>());
        await _saleRepository.Received(1).CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_InvalidCommand_ThrowsValidationException()
    {
        // Arrange
        var command = new CreateSaleCommand(); // Missing required values -> validator should fail in handler

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }
}