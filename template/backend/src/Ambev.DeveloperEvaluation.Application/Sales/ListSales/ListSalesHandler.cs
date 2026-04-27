using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Common.Data;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

public class ListSalesHandler : IRequestHandler<ListSalesCommand, PaginatedResult<ListSalesResult>>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public ListSalesHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<ListSalesResult>> Handle(ListSalesCommand command, CancellationToken cancellationToken)
    {
        var validator = new ListSalesValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var (sales, totalSales) = await _saleRepository.GetPagedAsync(command.Page, command.Size, command.Order, cancellationToken);

        var mappedResults = _mapper.Map<List<ListSalesResult>>(sales);

        return new PaginatedResult<ListSalesResult>(mappedResults, totalSales, command.Page, command.Size);
    }
}