using Ambev.DeveloperEvaluation.Application.Dtos;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Application.Users.ListUsers;

public class ListUserResult
{
    public Guid Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public NameDto Name { get; set; } = new();

    public AddressDto Address { get; set; } = new();

    public string Phone { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    public UserStatus Status { get; set; }
}