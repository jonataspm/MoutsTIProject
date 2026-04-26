using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;

public record CancelSaleItemCommand(Guid Id, Guid ItemId) : IRequest<bool>;