using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Testcontainers.MongoDb;
using Xunit;

namespace Ambev.DeveloperEvaluation.UnitTests.Fixtures;

public class MongoDbFixture : IAsyncLifetime
{
    static MongoDbFixture()
    {
        try
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.DateTime));
        }
        catch (BsonSerializationException)
        { }
    }

    private readonly MongoDbContainer _mongoDbContainer = new MongoDbBuilder()
        .WithImage("mongo:latest")
        .Build();

    public IMongoDatabase Database { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _mongoDbContainer.StartAsync();

        var client = new MongoClient(_mongoDbContainer.GetConnectionString());
        Database = client.GetDatabase("AmbevTestDb");
    }

    public async Task DisposeAsync()
    {
        await _mongoDbContainer.DisposeAsync();
    }

    /// <summary>
    /// Limpa a coleção antes de cada teste para garantir isolamento.
    /// </summary>
    public async Task ClearCollectionAsync(string collectionName)
    {
        await Database.DropCollectionAsync(collectionName);
    }
}