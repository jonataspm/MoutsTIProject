using AutoMapper;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Common.Data;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Carts.ListCarts;

public class ListCartsHandler : IRequestHandler<ListCartsCommand, PaginatedResult<ListCartsResult>>
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;

    public ListCartsHandler(ICartRepository cartRepository, IMapper mapper)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<ListCartsResult>> Handle(ListCartsCommand command, CancellationToken cancellationToken)
    {
        var validator = new ListCartsValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var (carts, totalUsers) = await _cartRepository.GetPagedAsync(command.Page, command.Size, command.Order, cancellationToken);

        var mappedResults = _mapper.Map<List<ListCartsResult>>(carts);
        var totalItems = carts.Count();
        var totalPages = command.Size == 0 ? 0 : (int)Math.Ceiling((double)totalUsers / command.Size);

        return new PaginatedResult<ListCartsResult>(mappedResults, totalItems, command.Page, command.Size);
    }
}