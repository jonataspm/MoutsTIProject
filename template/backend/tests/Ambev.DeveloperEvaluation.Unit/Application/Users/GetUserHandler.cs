using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.UnitTests.Application.Users;

public class GetUserHandlerTests
{
    private readonly IUserRepository _repositoryMock;
    private readonly IMapper _mapperMock;
    private readonly GetUserHandler _handler;

    public GetUserHandlerTests()
    {
        _repositoryMock = Substitute.For<IUserRepository>();
        _mapperMock = Substitute.For<IMapper>();
        _handler = new GetUserHandler(_repositoryMock, _mapperMock);
    }

    [Fact(DisplayName = "Deve retornar o utilizador mapeado quando o ID for encontrado")]
    public async Task Handle_UserExists_ShouldReturnResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Username = "testuser" };
        var expectedResult = new GetUserResult { Id = userId, Username = "testuser" };

        _repositoryMock.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);
        _mapperMock.Map<GetUserResult>(user).Returns(expectedResult);

        // Act
        var result = await _handler.Handle(new GetUserCommand(userId), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("testuser", result.Username);
    }
}