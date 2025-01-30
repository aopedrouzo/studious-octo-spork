using Microsoft.EntityFrameworkCore;
using FootballManager.Infrastructure.Data;
using FootballManager.Infrastructure.Data.Repositories;
using FootballManager.Domain.Entities;
using Xunit;

namespace FootballClubManagerTests.UnitTests.Infrastructure.Data.Repositories;

public class PostgreRepositoryTests
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

    private static Club CreateTestClub(
        string name = "Test FC",
        decimal budget = 1000000m)
    {
        return new Club(name, budget);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingEntity_ReturnsEntity()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new PostgreRepository<Club>(context);
        var club = CreateTestClub();
        await context.Clubs.AddAsync(club);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(club.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(club.Name, result.Name);
        Assert.Equal(club.Budget, result.Budget);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingEntity_ThrowsKeyNotFoundException()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new PostgreRepository<Club>(context);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => repository.GetByIdAsync(999));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllEntities()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new PostgreRepository<Club>(context);
        var initialClubs = await repository.GetAllAsync();
        var initialCount = initialClubs.Count();

        var clubs = new[]
        {
            CreateTestClub("Club 1"),
            CreateTestClub("Club 2"),
            CreateTestClub("Club 3")
        };
        await context.Clubs.AddRangeAsync(clubs);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Equal(initialCount + 3, result.Count());
    }

    [Fact]
    public async Task AddAsync_ValidEntity_AddsToDatabase()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new PostgreRepository<Club>(context);
        var club = CreateTestClub();

        // Act
        var result = await repository.AddAsync(club);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(0, result.Id);
        var savedClub = await context.Clubs.FindAsync(result.Id);
        Assert.NotNull(savedClub);
        Assert.Equal(club.Name, savedClub.Name);
    }

    [Fact]
    public async Task UpdateAsync_ExistingEntity_UpdatesDatabase()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new PostgreRepository<Club>(context);
        var club = CreateTestClub();
        await context.Clubs.AddAsync(club);
        await context.SaveChangesAsync();

        // Act
        club.ModifyBudget(500000m, 0);
        await repository.UpdateAsync(club);

        // Assert
        var updatedClub = await context.Clubs.FindAsync(club.Id);
        Assert.NotNull(updatedClub);
        Assert.Equal(1500000m, updatedClub.Budget);
    }

    [Fact]
    public async Task DeleteAsync_ExistingEntity_RemovesFromDatabase()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new PostgreRepository<Club>(context);
        var club = CreateTestClub();
        await context.Clubs.AddAsync(club);
        await context.SaveChangesAsync();

        // Act
        await repository.DeleteAsync(club);

        // Assert
        var deletedClub = await context.Clubs.FindAsync(club.Id);
        Assert.Null(deletedClub);
    }

    [Fact]
    public async Task ExistsAsync_ExistingEntity_ReturnsTrue()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new PostgreRepository<Club>(context);
        var club = CreateTestClub();
        await context.Clubs.AddAsync(club);
        await context.SaveChangesAsync();

        // Act
        var exists = await repository.ExistsAsync(club.Id);

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsAsync_NonExistingEntity_ReturnsFalse()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new PostgreRepository<Club>(context);

        // Act
        var exists = await repository.ExistsAsync(999);

        // Assert
        Assert.False(exists);
    }

    [Fact]
    public async Task AddRangeAsync_ValidEntities_AddsAllToDatabase()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new PostgreRepository<Club>(context);
        var initialClubs = await context.Clubs.ToListAsync();
        var initialCount = initialClubs.Count;

        var clubs = new[]
        {
            CreateTestClub("Club 1"),
            CreateTestClub("Club 2"),
            CreateTestClub("Club 3")
        };

        // Act
        await repository.AddRangeAsync(clubs);

        // Assert
        var savedClubs = await context.Clubs.ToListAsync();
        Assert.Equal(initialCount + 3, savedClubs.Count);
        Assert.Contains(savedClubs, c => c.Name == "Club 1");
        Assert.Contains(savedClubs, c => c.Name == "Club 2");
        Assert.Contains(savedClubs, c => c.Name == "Club 3");
    }
} 