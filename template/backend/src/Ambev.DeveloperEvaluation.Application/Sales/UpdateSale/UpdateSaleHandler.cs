using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateSaleHandler> _logger;

    public UpdateSaleHandler(ISaleRepository saleRepository, IMapper mapper, ILogger<UpdateSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var existingSale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (existingSale == null)
            throw new KeyNotFoundException($"Sale with ID {command.Id} not found");

        if (existingSale.IsCancelled)
            throw new InvalidOperationException("Cannot modify a cancelled sale.");

        existingSale.CustomerId = command.CustomerId;
        existingSale.CustomerName = command.CustomerName;
        existingSale.BranchId = command.BranchId;
        existingSale.BranchName = command.BranchName;
        existingSale.Date = command.Date;

        var itemsToRemove = existingSale.Items
            .Where(ei => !command.Items.Any(ri => ri.ProductId == ei.ProductId))
            .ToList();

        foreach (var itemToRemove in itemsToRemove)
        {
            itemToRemove.Cancel();
        }

        foreach (var itemRequest in command.Items)
        {
            var existingItem = existingSale.Items
                .FirstOrDefault(ei => ei.ProductId == itemRequest.ProductId);

            if (existingItem != null)
            {
                existingItem.UpdateQuantity(itemRequest.Quantity);
            }
            else
            {
                existingSale.Items.Add(new SaleItem(
                    itemRequest.ProductId,
                    itemRequest.ProductName,
                    itemRequest.Quantity,
                    itemRequest.UnitPrice));
            }
        }

        existingSale.CalculateTotal();

        var updatedSale = await _saleRepository.UpdateAsync(existingSale, cancellationToken);

        _logger.LogInformation("EVENTO PUBLICADO: SaleModified - Venda {SaleNumber} atualizada via Merge de Itens.", updatedSale.SaleNumber);

        return _mapper.Map<UpdateSaleResult>(updatedSale);
    }
}