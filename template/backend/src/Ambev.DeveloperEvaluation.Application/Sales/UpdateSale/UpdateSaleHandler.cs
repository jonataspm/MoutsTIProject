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

        existingSale.SaleNumber = command.SaleNumber;
        existingSale.Date = command.Date;
        existingSale.CustomerId = command.CustomerId;
        existingSale.CustomerName = command.CustomerName;
        existingSale.BranchId = command.BranchId;
        existingSale.BranchName = command.BranchName;

        existingSale.Items.Clear();
        foreach (var itemDto in command.Items)
        {
            existingSale.Items.Add(new SaleItem(itemDto.ProductId, itemDto.ProductName, itemDto.Quantity, itemDto.UnitPrice));
        }

        existingSale.CalculateTotal();

        var updatedSale = await _saleRepository.UpdateAsync(existingSale, cancellationToken);

        _logger.LogInformation("EVENTO PUBLICADO: SaleModified - A venda {SaleNumber} (ID: {SaleId}) foi MODIFICADA com sucesso. Novo Valor Total: {TotalAmount}",
            updatedSale.SaleNumber, updatedSale.Id, updatedSale.TotalAmount);

        return _mapper.Map<UpdateSaleResult>(updatedSale);
    }
}