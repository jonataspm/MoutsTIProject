using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

public interface ISaleRepository
{
    Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken);
    Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<(List<Sale> Data, int TotalCount)> GetPagedAsync(int page, int size, string order, CancellationToken cancellationToken);
    Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken); // Usado para exclusão física se necessário, mas as regras falam em "Cancel"
}