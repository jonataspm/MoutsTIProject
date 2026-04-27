using Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Bogus;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.UnitTests.Application.Auth;

public class AuthenticateUserHandlerTests
{
    private readonly IUserRepository _userRepositoryMock;
    private readonly IPasswordHasher _passwordHasherMock;
    private readonly IJwtTokenGenerator _jwtTokenGeneratorMock;
    private readonly AuthenticateUserHandler _handler;

    public AuthenticateUserHandlerTests()
    {
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _passwordHasherMock = Substitute.For<IPasswordHasher>();
        _jwtTokenGeneratorMock = Substitute.For<IJwtTokenGenerator>();

        _handler = new AuthenticateUserHandler(
            _userRepositoryMock,
            _passwordHasherMock,
            _jwtTokenGeneratorMock
        );
    }

    [Fact(DisplayName = "Deve autenticar com sucesso e retornar token quando as credenciais são válidas")]
    public async Task Handle_ValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var command = new AuthenticateUserCommand
        {
            Username = "usuario.teste",
            Password = "Password123!"
        };

        var user = new User
        {
            Username = command.Username,
            Password = "HashedPassword",
            Status = UserStatus.Active,
            Role = UserRole.Customer,
            Email = "teste@ambev.com.br"
        };

        _userRepositoryMock.GetByUserNameAsync(command.Username, Arg.Any<CancellationToken>())
            .Returns(user);

        _passwordHasherMock.VerifyPassword(command.Password, user.Password)
            .Returns(true);

        _jwtTokenGeneratorMock.GenerateToken(user)
            .Returns("fake-jwt-token");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("fake-jwt-token", result.Token);
        _jwtTokenGeneratorMock.Received(1).GenerateToken(user);
    }

    [Fact(DisplayName = "Deve lançar UnauthorizedAccessException quando o utilizador não existe")]
    public async Task Handle_UserNotFound_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var command = new AuthenticateUserCommand { Username = "inexistente", Password = "any" };
        _userRepositoryMock.GetByUserNameAsync(command.Username, Arg.Any<CancellationToken>())
            .Returns((User)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact(DisplayName = "Deve lançar UnauthorizedAccessException quando a password está incorreta")]
    public async Task Handle_WrongPassword_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var command = new AuthenticateUserCommand { Username = "user", Password = "wrong" };
        var user = new User { Username = "user", Password = "CorrectHash" };

        _userRepositoryMock.GetByUserNameAsync(command.Username, Arg.Any<CancellationToken>())
            .Returns(user);

        _passwordHasherMock.VerifyPassword(command.Password, user.Password)
            .Returns(false);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact(DisplayName = "Deve lançar UnauthorizedAccessException quando o utilizador está inativo")]
    public async Task Handle_InactiveUser_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var command = new AuthenticateUserCommand { Username = "inativo", Password = "password" };
        var user = new User
        {
            Username = "inativo",
            Password = "hash",
            Status = UserStatus.Inactive // Status que reprova na ActiveUserSpecification
        };

        _userRepositoryMock.GetByUserNameAsync(command.Username, Arg.Any<CancellationToken>())
            .Returns(user);

        _passwordHasherMock.VerifyPassword(command.Password, user.Password)
            .Returns(true);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }
}