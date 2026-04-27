using Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;
using Ambev.DeveloperEvaluation.Application.Dtos;
using Ambev.DeveloperEvaluation.Application.Users.CreateUser;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.WebApi;
using Ambev.DeveloperEvaluation.WebApi.Features.Auth.AuthenticateUserFeature;
using Ambev.DeveloperEvaluation.WebApi.Features.Users; // Ajuste para o namespace do seu Request
using Ambev.DeveloperEvaluation.WebApi.Features.Users.CreateUser;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Ambev.DeveloperEvaluation.FunctionalTests.Endpoints;

// Usamos a nossa Factory como Fixture para a classe
public class UserEndpointsTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public UserEndpointsTests(CustomWebApplicationFactory<Program> factory)
    {
        // Cria um HttpClient que se comunica diretamente com a API em memória
        _client = factory.CreateClient();
    }

    [Fact(DisplayName = "POST /api/users - Deve criar um usuário e retornar 201 Created")]
    public async Task PostUsers_ValidRequest_ShouldReturnCreated()
    {
        // Arrange - Simulando o Payload (JSON) que o Front-end enviaria
        var request = new CreateUserRequest
        {
            Username = "funcionario_ambev",
            Password = "Password123!",
            Email = "funcionario@ambev.com.br",
            Phone = "+5511999999999",
            Role = UserRole.Customer,
            Status = UserStatus.Active,
            Name = new NameDto { Firstname = "João", Lastname = "Silva" },
            Address = new AddressDto
            {
                City = "São Paulo",
                Street = "Av Paulista",
                Number = 1000,
                Zipcode = "01310-100",
                Geolocation = new GeolocationDto { Lat = "0", Long = "0" }
            }
        };

        // Act - Faz a chamada HTTP real para o Controller
        var response = await _client.PostAsJsonAsync("/api/users", request);

        // Assert - Valida o status HTTP
        response.EnsureSuccessStatusCode(); // Falha se não for 200-299
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        // Opcional: Validar o corpo da resposta
        var responseData = await response.Content.ReadFromJsonAsync<ApiResponseWithData<CreateUserResponse>>();
        Assert.NotNull(responseData);
        Assert.True(responseData.Success);
        Assert.NotEqual(Guid.Empty, responseData.Data.Id);
    }

    [Fact(DisplayName = "POST /api/auth/login - Deve autenticar usuário e retornar Token JWT")]
    public async Task PostAuthLogin_ValidCredentials_ShouldReturnToken()
    {
        // Arrange - Primeiro criamos o usuário (usando a própria API)
        var userRequest = new CreateUserRequest
        {
            Username = "admin_vendas",
            Password = "Jonatas01j.",
            Email = "admin@ambev.com.br",
            Phone = "11999999999",
            Role = UserRole.Admin,
            Status = UserStatus.Active,
            Name = new NameDto { Firstname = "Admin", Lastname = "User" },
            Address = new AddressDto { City = "SP", Street = "Rua", Number = 1, Zipcode = "00000-000", Geolocation = new GeolocationDto { Lat = "0", Long = "0" } }
        };
        var res = await _client.PostAsJsonAsync("/api/users", userRequest);

        var loginRequest = new AuthenticateUserRequest
        {
            Username = "admin_vendas", 
            Password = "Jonatas01j."
        };

        // Act - Chama o endpoint de login
        var response = await _client.PostAsJsonAsync("/api/auth", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        var jsonOptions = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var authResult = System.Text.Json.JsonSerializer.Deserialize<ResponseToken>(responseString, jsonOptions);

        Assert.NotNull(authResult?.Token);
    }

    private class ResponseToken
    {
        public string Token { get; set; }
    }

    private class ApiResponseWithData<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
