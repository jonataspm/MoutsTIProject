using Ambev.DeveloperEvaluation.ORM;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Extensions.Hosting;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional.TestFixtures;

public class CustomWebApplicationFactory : WebApplicationFactory<Ambev.DeveloperEvaluation.WebApi.Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.FirstOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<DefaultContext>));

            if (dbContextDescriptor != null)
                services.Remove(dbContextDescriptor);

            services.AddDbContext<DefaultContext>(options =>
            {
                options.UseInMemoryDatabase($"FunctionalTestDb_{Guid.NewGuid()}");
            });

            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DefaultContext>();
            context.Database.EnsureCreated();
        });

        builder.UseEnvironment("Testing");
    }
}

public class TestHttpClient
{
    private readonly HttpClient _httpClient;

    public TestHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpResponseMessage> GetAsync(string requestUri, string? bearerToken = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        if (!string.IsNullOrEmpty(bearerToken))
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

        return await _httpClient.SendAsync(request);
    }

    public async Task<HttpResponseMessage> PostAsync<T>(
        string requestUri,
        T? content = null,
        string? bearerToken = null)
        where T : class
    {
        var request = new HttpRequestMessage(HttpMethod.Post, requestUri);

        if (content != null)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(content);
            request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        }

        if (!string.IsNullOrEmpty(bearerToken))
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

        return await _httpClient.SendAsync(request);
    }

    public async Task<HttpResponseMessage> PutAsync<T>(
        string requestUri,
        T? content = null,
        string? bearerToken = null)
        where T : class
    {
        var request = new HttpRequestMessage(HttpMethod.Put, requestUri);

        if (content != null)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(content);
            request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        }

        if (!string.IsNullOrEmpty(bearerToken))
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

        return await _httpClient.SendAsync(request);
    }

    public async Task<HttpResponseMessage> DeleteAsync(string requestUri, string? bearerToken = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, requestUri);
        if (!string.IsNullOrEmpty(bearerToken))
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

        return await _httpClient.SendAsync(request);
    }

    public async Task<T?> GetContentAsync<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrEmpty(content))
            return default;

        return System.Text.Json.JsonSerializer.Deserialize<T>(content,
            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }
}

public abstract class FunctionalTestBase : IAsyncLifetime
{
    protected readonly CustomWebApplicationFactory Factory;
    protected TestHttpClient Client { get; private set; } = null!;
    protected DefaultContext DbContext { get; private set; } = null!;

    protected FunctionalTestBase()
    {
        Factory = new CustomWebApplicationFactory();
    }

    public async Task InitializeAsync()
    {
        var httpClient = Factory.CreateClient();
        Client = new TestHttpClient(httpClient);

        // Obtém DbContext para seeding de dados
        var scope = Factory.Services.CreateScope();
        DbContext = scope.ServiceProvider.GetRequiredService<DefaultContext>();
        await DbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await DbContext.Database.EnsureDeletedAsync();
        DbContext.Dispose();
        Factory.Dispose();
    }

    protected async Task SeedAsync(Func<DefaultContext, Task> seedFunc)
    {
        await seedFunc(DbContext);
        await DbContext.SaveChangesAsync();
    }

    protected string GenerateMockJwtToken()
    {
        return string.Empty;
    }
}
