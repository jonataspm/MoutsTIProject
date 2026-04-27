using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Ambev.DeveloperEvaluation.UnitTests.Fixtures;
using Xunit;

namespace Ambev.DeveloperEvaluation.UnitTests.Integration.Repositories;

public class UserRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly UserRepository _repository;
    private readonly DatabaseFixture _fixture;

    public UserRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _repository = new UserRepository(_fixture.Context);
        _fixture.ClearDatabase();
    }

    [Fact(DisplayName = "Deve salvar um usuário e recuperá-lo por e-mail")]
    public async Task CreateAndGetByEmail_ShouldWorkCorrectly()
    {
        // Arrange
        var user = new User { Username = "tester", Email = "test@ambev.com", Password = "123" };

        // Act
        await _repository.CreateAsync(user, default);
        var retrieved = await _repository.GetByEmailAsync("test@ambev.com", default);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(user.Username, retrieved.Username);
    }

    [Fact(DisplayName = "Deve recuperar um usuário por Username")]
    public async Task GetByUserName_ShouldReturnUser()
    {
        // Arrange
        var user = new User { Username = "joao_dev", Email = "joao@ambev.com", Password = "123" };
        await _repository.CreateAsync(user, default);

        // Act
        var retrieved = await _repository.GetByUserNameAsync("joao_dev", default);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(user.Email, retrieved.Email);
    }
}