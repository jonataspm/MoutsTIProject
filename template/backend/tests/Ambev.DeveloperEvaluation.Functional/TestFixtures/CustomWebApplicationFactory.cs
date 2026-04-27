using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Mapping.Mongo;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System.Linq;

namespace Ambev.DeveloperEvaluation.FunctionalTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.Sources.Clear(); 
            config.AddJsonFile("appsettings.Testing.json", optional: false, reloadOnChange: true);
        });

        builder.ConfigureServices((context, services) =>
        {
            var configuration = context.Configuration;

            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<DefaultContext>));

            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<DefaultContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForFunctionalTesting");
                options.ConfigureWarnings(x => x.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning));
            });

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<DefaultContext>();
            db.Database.EnsureCreated();

            try
            {
                BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
                BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.DateTime));
            }
            catch (BsonSerializationException)
            { }
            
            var mongoConnection = configuration.GetSection("MongoDbSettings:ConnectionString").Value;
            var mongoDatabase = configuration.GetSection("MongoDbSettings:Database").Value;

            services.AddSingleton<IMongoClient>(new MongoClient(mongoConnection));
            services.AddScoped<IMongoDatabase>(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                return client.GetDatabase(mongoDatabase);
            });

            CartMap.Configure();
        });
    }
}