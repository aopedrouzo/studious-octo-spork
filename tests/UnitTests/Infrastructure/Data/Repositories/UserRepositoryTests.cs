using Microsoft.EntityFrameworkCore;
using FootballManager.Infrastructure.Data;
using FootballManager.Infrastructure.Data.Repositories;
using FootballManager.Domain.Entities;
using Xunit;

namespace FootballClubManagerTests.UnitTests.Infrastructure.Data.Repositories;

public class UserRepositoryTests
{
    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    private static User CreateTestUser(
        string username = "testuser",
        string role = "User",
        string passwordHash = "hashedpassword123")
    {
        return new User
        {
            Username = username,
            Role = role,
            PasswordHash = passwordHash,
            RefreshTokens = new List<RefreshToken>()
        };
    }

    private static RefreshToken CreateTestRefreshToken(
        string token = "testtoken123",
        string username = "testuser")
    {
        return new RefreshToken
        {
            Token = token,
            Username = username,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };
    }

    [Fact]
    public async Task GetByUsernameAsync_ExistingUser_ReturnsUser()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new UserRepository(context);
        var user = CreateTestUser();
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByUsernameAsync(user.Username);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Username, result.Username);
        Assert.Equal(user.Role, result.Role);
        Assert.Equal(user.PasswordHash, result.PasswordHash);
    }

    [Fact]
    public async Task GetByUsernameAsync_NonExistingUser_ReturnsNull()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new UserRepository(context);

        // Act
        var result = await repository.GetByUsernameAsync("nonexistent");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetRefreshTokenAsync_ExistingToken_ReturnsRefreshToken()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new UserRepository(context);
        var user = CreateTestUser();
        var refreshToken = CreateTestRefreshToken(username: user.Username);
        
        await context.Users.AddAsync(user);
        await context.RefreshTokens.AddAsync(refreshToken);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetRefreshTokenAsync(user.Username, refreshToken.Token);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(refreshToken.Token, result.Token);
        Assert.Equal(refreshToken.Username, result.Username);
    }

    [Fact]
    public async Task GetRefreshTokenAsync_NonExistingToken_ReturnsNull()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new UserRepository(context);
        var user = CreateTestUser();
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetRefreshTokenAsync(user.Username, "nonexistenttoken");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetRefreshTokenAsync_WrongUsername_ReturnsNull()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new UserRepository(context);
        var user = CreateTestUser();
        var refreshToken = CreateTestRefreshToken(username: user.Username);
        
        await context.Users.AddAsync(user);
        await context.RefreshTokens.AddAsync(refreshToken);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetRefreshTokenAsync("wrongusername", refreshToken.Token);

        // Assert
        Assert.Null(result);
    }

    // Base Repository Tests
    [Fact]
    public async Task AddAsync_ValidUser_AddsToDatabase()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new UserRepository(context);
        var user = CreateTestUser();

        // Act
        var result = await repository.AddAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(0, result.Id);
        var savedUser = await context.Users.FindAsync(result.Id);
        Assert.NotNull(savedUser);
        Assert.Equal(user.Username, savedUser.Username);
    }

    [Fact]
    public async Task UpdateAsync_ExistingUser_UpdatesDatabase()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new UserRepository(context);
        var user = CreateTestUser();
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        // Act
        user.Role = "Admin";
        await repository.UpdateAsync(user);

        // Assert
        var updatedUser = await context.Users.FindAsync(user.Id);
        Assert.NotNull(updatedUser);
        Assert.Equal("Admin", updatedUser.Role);
    }

    [Fact]
    public async Task DeleteAsync_ExistingUser_RemovesFromDatabase()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new UserRepository(context);
        var user = CreateTestUser();
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        // Act
        await repository.DeleteAsync(user);

        // Assert
        var deletedUser = await context.Users.FindAsync(user.Id);
        Assert.Null(deletedUser);
    }
} 