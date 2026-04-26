using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Entities.Counters;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _context;

    public SaleRepository(DefaultContext context)
    {
        _context = context;
    }

    public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken)
    {
        await _context.Sales.AddAsync(sale, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<(List<Sale> Data, int TotalCount)> GetPagedAsync(int page, int size, string order, CancellationToken cancellationToken)
    {
        var query = _context.Sales.Include(s => s.Items).AsQueryable();

        var total = await query.CountAsync(cancellationToken);
        var data = await query.Skip((page - 1) * size).Take(size).ToListAsync(cancellationToken);

        return (data, total);
    }

    public async Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken)
    {
        _context.Sales.Update(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var sale = await GetByIdAsync(id, cancellationToken);
        if (sale == null) return false;

        _context.Sales.Remove(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<string> GenerateNextSaleNumberAsync(CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var datePrefix = today.ToString("yyyyMMdd");

        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var counter = await _context.SaleCounters
                .FirstOrDefaultAsync(c => c.Date == today, cancellationToken);

            int nextNumber;

            if (counter == null)
            {
                nextNumber = 1;
                counter = new SaleCounter { Date = today, LastNumber = nextNumber };
                await _context.SaleCounters.AddAsync(counter, cancellationToken);
            }
            else
            {
                counter.LastNumber += 1;
                nextNumber = counter.LastNumber;
                _context.SaleCounters.Update(counter);
            }

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return $"{datePrefix}{nextNumber:D4}";
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}