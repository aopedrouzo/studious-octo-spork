using Microsoft.EntityFrameworkCore;
using FootballManager.Infrastructure.Data;
using FootballManager.Infrastructure.Data.Repositories;
using FootballManager.Domain.Entities;
using FootballManager.Domain.Enums;
using FootballManager.Application.Common;
using Xunit;

namespace FootballClubManagerTests.UnitTests.Infrastructure.Data.Repositories;

public class PlayerRepositoryTests
{
    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        context.Database.EnsureCreated();
        
        // Clear all existing data
        context.Players.RemoveRange(context.Players);
        context.Coaches.RemoveRange(context.Coaches);
        context.Clubs.RemoveRange(context.Clubs);
        context.Users.RemoveRange(context.Users);
        context.SaveChanges();
        
        return context;
    }

    private static Player CreateTestPlayer(
        string firstName = "John",
        string lastName = "Doe",
        Position position = Position.Forward,
        decimal salary = 50000m,
        int? clubId = null)
    {
        var player = new Player(
            firstName,
            lastName,
            DateOnly.FromDateTime(DateTime.Now),
            position,
            salary,
            $"{firstName.ToLower()}.{lastName.ToLower()}@test.com"
        );

        if (clubId.HasValue)
        {
            player.GetType().GetProperty("ClubId")?.SetValue(player, clubId);
        }

        return player;
    }

    [Fact]
    public async Task GetFilteredPlayersAsync_NoFilters_ReturnsAllPlayers()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new PlayerRepository(context);
        var players = new[]
        {
            CreateTestPlayer("John", "Doe"),
            CreateTestPlayer("Jane", "Smith"),
            CreateTestPlayer("Bob", "Wilson")
        };
        await context.Players.AddRangeAsync(players);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetFilteredPlayersAsync();

        // Assert
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(3, result.Items.Count());
    }

    [Fact]
    public async Task GetFilteredPlayersAsync_ByClubId_ReturnsFilteredPlayers()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new PlayerRepository(context);
        var players = new[]
        {
            CreateTestPlayer(clubId: 1),
            CreateTestPlayer(clubId: 1),
            CreateTestPlayer(clubId: 2)
        };
        await context.Players.AddRangeAsync(players);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetFilteredPlayersAsync(clubId: 1);

        // Assert
        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Items, p => Assert.Equal(1, p.ClubId));
    }


    [Fact]
    public async Task GetFilteredPlayersAsync_ByPosition_ReturnsMatchingPlayers()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new PlayerRepository(context);
        var players = new[]
        {
            CreateTestPlayer(position: Position.Forward),
            CreateTestPlayer(position: Position.Midfielder),
            CreateTestPlayer(position: Position.Forward)
        };
        await context.Players.AddRangeAsync(players);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetFilteredPlayersAsync(position: Position.Forward);

        // Assert
        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Items, p => Assert.Equal(Position.Forward, p.Position));
    }

    [Fact]
    public async Task GetFilteredPlayersAsync_BySalaryRange_ReturnsMatchingPlayers()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new PlayerRepository(context);
        var players = new[]
        {
            CreateTestPlayer(salary: 40000m),
            CreateTestPlayer(salary: 50000m),
            CreateTestPlayer(salary: 60000m)
        };
        await context.Players.AddRangeAsync(players);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetFilteredPlayersAsync(minSalary: 45000m, maxSalary: 55000m);

        // Assert
        Assert.Equal(1, result.TotalCount);
        Assert.All(result.Items, p => Assert.Equal(50000m, p.Salary));
    }

    [Fact]
    public async Task GetFilteredPlayersAsync_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new PlayerRepository(context);
        var players = new[]
        {
            CreateTestPlayer("Player", "One"),
            CreateTestPlayer("Player", "Two"),
            CreateTestPlayer("Player", "Three"),
            CreateTestPlayer("Player", "Four"),
            CreateTestPlayer("Player", "Five")
        };
        await context.Players.AddRangeAsync(players);
        await context.SaveChangesAsync();

        var paginationParams = new PaginationParams { PageNumber = 2, PageSize = 2 };

        // Act
        var result = await repository.GetFilteredPlayersAsync(paginationParams: paginationParams);

        // Assert
        Assert.Equal(5, result.TotalCount);
        Assert.Equal(2, result.Items.Count());
        Assert.Equal(2, result.PageNumber);
    }

    [Fact]
    public async Task GetFilteredPlayersAsync_CombinedFilters_ReturnsMatchingPlayers()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new PlayerRepository(context);
        var players = new[]
        {
            CreateTestPlayer("John", "Doe", Position.Forward, 50000m, 1),
            CreateTestPlayer("Jane", "Smith", Position.Forward, 40000m, 1),
            CreateTestPlayer("John", "Smith", Position.Midfielder, 60000m, 2)
        };
        await context.Players.AddRangeAsync(players);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetFilteredPlayersAsync(
            clubId: 1,
            position: Position.Forward,
            minSalary: 45000m
        );

        // Assert
        Assert.Equal(1, result.TotalCount);
        var player = Assert.Single(result.Items);
        Assert.Equal("John", player.FirstName);
        Assert.Equal("Doe", player.LastName);
        Assert.Equal(Position.Forward, player.Position);
        Assert.Equal(50000m, player.Salary);
        Assert.Equal(1, player.ClubId);
    }
} 