using Ambev.DeveloperEvaluation.Application.Dtos;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.WebApi;
using Ambev.DeveloperEvaluation.WebApi.Features.Auth.AuthenticateUserFeature;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.CreateUser;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Ambev.DeveloperEvaluation.FunctionalTests.Endpoints;

public class ProductEndpointsTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ProductEndpointsTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<string> GetTokenForRoleAsync(UserRole role, string username)
    {
        var password = "StrongPassword123!";

        var userRequest = new CreateUserRequest
        {
            Username = username,
            Password = password,
            Email = $"{username}@ambev.com",
            Phone = "11999999999",
            Role = role,
            Status = UserStatus.Active,
            Name = new NameDto { Firstname = "Test", Lastname = "User" },
            Address = new AddressDto { City = "SP", Street = "Rua", Number = 1, Zipcode = "00000-000", Geolocation = new GeolocationDto { Lat = "0", Long = "0" } }
        };
        await _client.PostAsJsonAsync("/api/users", userRequest);

        var loginRequest = new AuthenticateUserRequest { Username = username, Password = password };
        var response = await _client.PostAsJsonAsync("/api/auth", loginRequest);

        var responseString = await response.Content.ReadAsStringAsync();
        var authResult = JsonSerializer.Deserialize<ResponseToken>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return authResult!.Token;
    }


    [Fact(DisplayName = "POST /api/products - Deve retornar 401 Unauthorized se não enviar Token")]
    public async Task PostProducts_WithoutToken_ShouldReturnUnauthorized()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;
        var request = new { Title = "Cerveja Nova", Price = 10.50m, Category = "Cervejas" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/products", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(DisplayName = "POST /api/products - Deve retornar 403 Forbidden se for usuário comum (Customer)")]
    public async Task PostProducts_WithCustomerToken_ShouldReturnForbidden()
    {
        // Arrange 
        var token = await GetTokenForRoleAsync(UserRole.Customer, "cliente_comum");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new { Title = "Cerveja Nova", Price = 10.50m, Category = "Cervejas" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/products", request);

        // Assert 
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact(DisplayName = "POST /api/products - Deve criar produto e retornar 201 Created se for Admin")]
    public async Task PostProducts_WithAdminToken_ShouldReturnCreated()
    {

        var token = await GetTokenForRoleAsync(UserRole.Admin, "admin_produtos");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new
        {
            Title = "Brahma Duplo Malte 330ml",
            Price = 4.50m,
            Description = "Cerveja Puro Malte",
            Category = "Cervejas",
            Image = "url_da_imagem",
            Rating = new { Rate = 4.5, Count = 100 }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/products", request);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact(DisplayName = "GET /api/products - Deve listar produtos mesmo sem estar logado (Público)")]
    public async Task GetProducts_PublicAccess_ShouldReturnOk()
    {
        // Arrange 
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.GetAsync("/api/products");

        // Assert 
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private class ResponseToken
    {
        public string Token { get; set; }
    }
}