using Ambev.DeveloperEvaluation.Application.Users.DeleteUser;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.UnitTests.Application.Users;

public class DeleteUserHandlerTests
{
    private readonly IUserRepository _repositoryMock;
    private readonly DeleteUserHandler _handler;

    public DeleteUserHandlerTests()
    {
        _repositoryMock = Substitute.For<IUserRepository>();
        _handler = new DeleteUserHandler(_repositoryMock);
    }

    [Fact(DisplayName = "Deve remover o utilizador quando o comando for válido")]
    public async Task Handle_ValidId_ShouldCallDeleteOnRepository()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _repositoryMock.DeleteAsync(userId, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await _handler.Handle(new DeleteUserCommand(userId), CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        await _repositoryMock.Received(1).DeleteAsync(userId, Arg.Any<CancellationToken>());
    }
}