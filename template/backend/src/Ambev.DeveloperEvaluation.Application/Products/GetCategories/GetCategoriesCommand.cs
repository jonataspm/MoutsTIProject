using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.GetCategories;

public record GetCategoriesCommand : IRequest<List<string>>;