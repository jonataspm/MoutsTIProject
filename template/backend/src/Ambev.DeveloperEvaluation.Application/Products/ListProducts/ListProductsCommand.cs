using MediatR;
using Ambev.DeveloperEvaluation.Common.Data;

namespace Ambev.DeveloperEvaluation.Application.Products.ListProducts;

public record ListProductsCommand : IRequest<PaginatedResult<ListProductsResult>>
{
    public int Page { get; init; } = 1;
    public int Size { get; init; } = 10;
    public string Order { get; init; } = string.Empty;
}