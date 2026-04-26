using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly DefaultContext _context;

    public ProductRepository(DefaultContext context)
    {
        _context = context;
    }

    public async Task<Product> CreateAsync(Product product, CancellationToken cancellationToken)
    {
        await _context.Products.AddAsync(product, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<List<Product>> GetAllAsync(int page, int size, string order, CancellationToken cancellationToken)
    {
        return await _context.Products
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);
    }

    public async Task<Product> UpdateAsync(Product product, CancellationToken cancellationToken)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = await GetByIdAsync(id, cancellationToken);
        if (product == null) return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<List<string>> GetCategoriesAsync(CancellationToken cancellationToken)
    {
        return await _context.Products
            .Select(p => p.Category)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Product>> GetByCategoryAsync(string category, int page, int size, string order, CancellationToken cancellationToken)
    {
        return await _context.Products
            .Where(p => p.Category.ToLower() == category.ToLower())
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);
    }

    public async Task<(List<Product> Data, int TotalCount)> GetPagedAsync(int page, int size, string order, CancellationToken cancellationToken)
    {
        var query = _context.Products.AsQueryable();
        var total = await query.CountAsync(cancellationToken);
        var data = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        return (data, total); 
    }

    public async Task<(List<Product> Data, int TotalCount)> GetPagedByCategoryAsync(string category, int page, int size, string order, CancellationToken cancellationToken)
    {
        var query = _context.Products.Where(p => p.Category.ToLower() == category.ToLower());

        var total = await query.CountAsync(cancellationToken);
        var data = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        return (data, total);
    }
}