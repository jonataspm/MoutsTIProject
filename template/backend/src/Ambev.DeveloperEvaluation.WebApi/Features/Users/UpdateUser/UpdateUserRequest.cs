using Ambev.DeveloperEvaluation.Application.Dtos;
using Ambev.DeveloperEvaluation.Domain.Enums;
using System.Text.Json.Serialization;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.UpdateUser
{
    public record UpdateUserRequest
    {
        [JsonIgnore]
        public Guid Id { get; init; }
        public string Username { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public string Phone { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public UserStatus Status { get; init; }
        public UserRole Role { get; init; }
        public NameDto Name { get; init; } = new();
        public AddressDto Address { get; init; } = new();
    }
}
