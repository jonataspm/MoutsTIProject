using Ambev.DeveloperEvaluation.Common.Extensions;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MongoDB.Driver;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class CartRepositoryMongo : ICartRepository
{
    private readonly IMongoCollection<Cart> _cartsCollection;

    public CartRepositoryMongo(IMongoDatabase mongoDatabase)
    {
        _cartsCollection = mongoDatabase.GetCollection<Cart>("Carts");
    }

    public async Task<Cart> CreateAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        await _cartsCollection.InsertOneAsync(cart, cancellationToken: cancellationToken);
        return cart;
    }

    public async Task<Cart?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Cart>.Filter.Eq(c => c.Id, id);
        return await _cartsCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Cart> UpdateAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Cart>.Filter.Eq(c => c.Id, cart.Id);

        await _cartsCollection.ReplaceOneAsync(filter, cart, cancellationToken: cancellationToken);

        return cart;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Cart>.Filter.Eq(c => c.Id, id);
        var result = await _cartsCollection.DeleteOneAsync(filter, cancellationToken);

        return result.DeletedCount > 0;
    }

    public async Task<(List<Cart> Carts, int TotalCarts)> GetPagedAsync(int page, int size, string order, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Cart>.Filter.Empty;
        var totalCarts = await _cartsCollection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        var sortDefinitions = new List<SortDefinition<Cart>>();

        if (!string.IsNullOrWhiteSpace(order))
        {
            var orderPairs = order.Split(',', StringSplitOptions.RemoveEmptyEntries);

            foreach (var pair in orderPairs)
            {
                var parts = pair.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var property = parts[0].CapitalizeFirst();
                var direction = parts.Length > 1 && parts[1].ToUpper() == "DESC" ? -1 : 1;

                sortDefinitions.Add(new JsonSortDefinition<Cart>($"{{ \"{property}\": {direction} }}"));
            }
        }

        var finalSort = sortDefinitions.Count > 0
            ? Builders<Cart>.Sort.Combine(sortDefinitions)
            : Builders<Cart>.Sort.Descending(c => c.Date);

        var carts = await _cartsCollection.Find(filter)
            .Sort(finalSort)
            .Skip((page - 1) * size)
            .Limit(size)
            .ToListAsync(cancellationToken);

        return (carts, (int)totalCarts);
    }
}