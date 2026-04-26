using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Common.Data;

namespace Ambev.DeveloperEvaluation.Application.Products.ListProductsByCategory;

public class ListProductsByCategoryHandler : IRequestHandler<ListProductsByCategoryCommand, PaginatedResult<ListProductsByCategoryResult>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public ListProductsByCategoryHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<ListProductsByCategoryResult>> Handle(ListProductsByCategoryCommand command, CancellationToken cancellationToken)
    {
        var validator = new ListProductsByCategoryValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var (products, totalProducts) = await _productRepository.GetPagedByCategoryAsync(command.Category, command.Page, command.Size, command.Order, cancellationToken);

        var mappedResults = _mapper.Map<List<ListProductsByCategoryResult>>(products);

        return new PaginatedResult<ListProductsByCategoryResult>(mappedResults, totalProducts, command.Page, command.Size);
    }
}