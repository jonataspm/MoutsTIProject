using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Products.GetCategories;

public class GetCategoriesHandler : IRequestHandler<GetCategoriesCommand, List<string>>
{
    private readonly IProductRepository _productRepository;

    public GetCategoriesHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<List<string>> Handle(GetCategoriesCommand command, CancellationToken cancellationToken)
    {
        return await _productRepository.GetCategoriesAsync(cancellationToken);
    }
}