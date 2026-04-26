using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

public interface ICartRepository
{
    Task<Cart> CreateAsync(Cart cart, CancellationToken cancellationToken);
    Task<Cart?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<(List<Cart> Carts, int TotalCarts)> GetPagedAsync(int page, int size, string order, CancellationToken cancellationToken);
    Task<Cart> UpdateAsync(Cart cart, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}