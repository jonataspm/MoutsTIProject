using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

public interface IProductRepository
{
    Task<Product> CreateAsync(Product product, CancellationToken cancellationToken);
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Product> UpdateAsync(Product product, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<(List<Product> Data, int TotalCount)> GetPagedAsync(int page, int size, string order, CancellationToken cancellationToken);
    Task<(List<Product> Data, int TotalCount)> GetPagedByCategoryAsync(string category, int page, int size, string order, CancellationToken cancellationToken);
    Task<List<string>> GetCategoriesAsync(CancellationToken cancellationToken);
}