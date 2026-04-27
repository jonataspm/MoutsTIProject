using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Ambev.DeveloperEvaluation.Domain.Common;

public abstract class BaseEntityMongo
{
    [BsonId]
    public Guid Id { get; set; } = Guid.NewGuid();
}