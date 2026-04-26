using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;

public class CancelSaleItemHandler : IRequestHandler<CancelSaleItemCommand, bool>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ILogger<CancelSaleItemHandler> _logger;

    public CancelSaleItemHandler(ISaleRepository saleRepository, ILogger<CancelSaleItemHandler> logger)
    {
        _saleRepository = saleRepository;
        _logger = logger;
    }

    public async Task<bool> Handle(CancelSaleItemCommand command, CancellationToken cancellationToken)
    {
        var validator = new CancelSaleItemValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);

        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {command.Id} not found");

        if (sale.IsCancelled)
            throw new InvalidOperationException("This sale is already cancelled.");

        sale.CancelItem(command.ItemId);

        await _saleRepository.UpdateAsync(sale, cancellationToken);

        _logger.LogInformation("EVENTO PUBLICADO: ItemCancelled - O Item {ItemId} da venda {SaleNumber} (ID: {SaleId}) foi CANCELADO. Novo Total: {TotalAmount}",
                    command.ItemId, sale.SaleNumber, sale.Id, sale.TotalAmount);

        return true;
    }
}