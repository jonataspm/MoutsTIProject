using AutoMapper;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Carts.GetCart;

public class GetCartHandler : IRequestHandler<GetCartCommand, GetCartResult>
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;

    public GetCartHandler(ICartRepository cartRepository, IMapper mapper)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
    }

    public async Task<GetCartResult> Handle(GetCartCommand command, CancellationToken cancellationToken)
    {
        var validator = new GetCartValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var cart = await _cartRepository.GetByIdAsync(command.Id, cancellationToken);

        if (cart == null)
            throw new KeyNotFoundException($"Cart with ID {command.Id} not found");

        return _mapper.Map<GetCartResult>(cart);
    }
}