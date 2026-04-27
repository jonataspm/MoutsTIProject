using Ambev.DeveloperEvaluation.Application.Carts.CreateCart;
using Ambev.DeveloperEvaluation.Application.Carts.Dtos;
using Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.WebApi;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Carts.CreateCart;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Ambev.DeveloperEvaluation.FunctionalTests.Endpoints;

public class CartEndpointsTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public CartEndpointsTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    private async Task<string> GetAdminTokenAsync()
    {
        var loginRequest = new { Username = "admin", Password = "Password123!" };
        var response = await _client.PostAsJsonAsync("/api/auth", loginRequest);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ResponseToken>(content, _jsonOptions);
        return result!.Token;
    }

    [Fact(DisplayName = "Fluxo Completo: Deve criar, buscar e atualizar um carrinho no MongoDB")]
    public async Task Cart_FullLifecycle_ShouldWorkCorrectly()
    {
        // 1. Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var userId = Guid.NewGuid();
        var createRequest = new CreateCartCommand
        {
            UserId = userId,
            Date = DateTime.UtcNow,
            Products = new List<CartProductDto> { new() { ProductId = Guid.NewGuid(), Quantity = 2 } }
        };

        // 2. Act
        var createResponse = await _client.PostAsJsonAsync("/api/carts", createRequest);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponseWithData<CreateCartResponse>>(_jsonOptions);
        var cartId = createResult!.Data.Id;

        // 3. Act
        var getResponse = await _client.GetAsync($"/api/carts/{cartId}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        // 4. Act
        var updateRequest = new UpdateCartCommand
        {
            Id = cartId,
            UserId = userId,
            Date = DateTime.UtcNow,
            Products = new List<CartProductDto>
            {
                new() { ProductId = Guid.NewGuid(), Quantity = 10 }
            }
        };
        var updateResponse = await _client.PutAsJsonAsync($"/api/carts/{cartId}", updateRequest);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        // 5. Assert Final 
        var finalGetResponse = await _client.GetAsync($"/api/carts/{cartId}");
        var finalContent = await finalGetResponse.Content.ReadAsStringAsync();
        Assert.Contains("10", finalContent); // Verifica se a nova quantidade está lá
    }

    [Fact(DisplayName = "POST /api/carts - Deve retornar 401 se tentar criar sem token")]
    public async Task CreateCart_NoToken_ShouldReturnUnauthorized()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        var response = await _client.PostAsJsonAsync("/api/carts", new { });
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private class ResponseToken
    {
        public string Token { get; set; }
    }
}