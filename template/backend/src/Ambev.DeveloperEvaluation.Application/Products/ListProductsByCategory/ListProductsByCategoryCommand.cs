using MediatR;
using Ambev.DeveloperEvaluation.Common.Data;

namespace Ambev.DeveloperEvaluation.Application.Products.ListProductsByCategory;

public record ListProductsByCategoryCommand : IRequest<PaginatedResult<ListProductsByCategoryResult>>
{
    public string Category { get; init; } = string.Empty;
    public int Page { get; init; } = 1;
    public int Size { get; init; } = 10;
    public string Order { get; init; } = string.Empty;
}