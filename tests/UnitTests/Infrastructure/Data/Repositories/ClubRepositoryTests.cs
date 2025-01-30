using Microsoft.EntityFrameworkCore;
using FootballManager.Infrastructure.Data;
using FootballManager.Infrastructure.Data.Repositories;
using FootballManager.Domain.Entities;
using FootballManager.Domain.Enums;
using Xunit;

namespace FootballClubManagerTests.UnitTests.Infrastructure.Data.Repositories;

public class ClubRepositoryTests
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

    private static Player CreateTestPlayer(
        string firstName = "John",
        string lastName = "Doe",
        decimal salary = 50000m)
    {
        return new Player(
            firstName,
            lastName,
            DateOnly.FromDateTime(DateTime.Now),
            Position.Forward,
            salary,
            "john.doe@test.com"
        );
    }

    private static Coach CreateTestCoach(
        string firstName = "Jane",
        string lastName = "Smith",
        decimal salary = 75000m)
    {
        return new Coach(
            firstName,
            lastName,
            DateOnly.FromDateTime(DateTime.Now),
            salary,
            "jane.smith@test.com"
        );
    }

    [Fact]
    public async Task GetTotalSalaryAsync_WithNoEmployees_ReturnsZero()
    {
        // Arrange
        using var context = CreateContext();
        var club = CreateTestClub();
        context.Clubs.Add(club);
        await context.SaveChangesAsync();

        var repository = new ClubRepository(context);

        // Act
        var totalSalary = await repository.GetTotalSalaryAsync(club.Id);

        // Assert
        Assert.Equal(0, totalSalary);
    }

    [Fact]
    public async Task GetTotalSalaryAsync_WithPlayersAndCoaches_ReturnsTotalSalary()
    {
        // Arrange
        using var context = CreateContext();
        var club = CreateTestClub();
        var player1 = CreateTestPlayer(salary: 50000m);
        var player2 = CreateTestPlayer("Jane", "Doe", 60000m);
        var coach = CreateTestCoach(salary: 90000m);

        club.Players.Add(player1);
        club.Players.Add(player2);
        club.Coaches.Add(coach);
        
        context.Clubs.Add(club);
        await context.SaveChangesAsync();

        var repository = new ClubRepository(context);

        // Act
        var totalSalary = await repository.GetTotalSalaryAsync(club.Id);

        // Assert
        var expectedTotal = 200000m; // 50000 + 60000 + 90000
        Assert.Equal(expectedTotal, totalSalary);
    }

    [Fact]
    public async Task GetTotalSalaryAsync_NonExistentClub_ThrowsKeyNotFoundException()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new ClubRepository(context);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => repository.GetTotalSalaryAsync(999));
    }

    [Fact]
    public async Task GetTotalSalaryAsync_WithOnlyPlayers_ReturnsPlayersSalarySum()
    {
        // Arrange
        using var context = CreateContext();
        var club = CreateTestClub();
        var player1 = CreateTestPlayer(salary: 50000m);
        var player2 = CreateTestPlayer("Jane", "Doe", 60000m);

        club.Players.Add(player1);
        club.Players.Add(player2);
        
        context.Clubs.Add(club);
        await context.SaveChangesAsync();

        var repository = new ClubRepository(context);

        // Act
        var totalSalary = await repository.GetTotalSalaryAsync(club.Id);

        // Assert
        var expectedTotal = 110000m; // 50000 + 60000
        Assert.Equal(expectedTotal, totalSalary);
    }

    [Fact]
    public async Task GetTotalSalaryAsync_WithOnlyCoaches_ReturnsCoachesSalarySum()
    {
        // Arrange
        using var context = CreateContext();
        var club = CreateTestClub();
        var coach1 = CreateTestCoach(salary: 75000m);
        var coach2 = CreateTestCoach("Bob", "Wilson", 85000m);

        club.Coaches.Add(coach1);
        club.Coaches.Add(coach2);
        
        context.Clubs.Add(club);
        await context.SaveChangesAsync();

        var repository = new ClubRepository(context);

        // Act
        var totalSalary = await repository.GetTotalSalaryAsync(club.Id);

        // Assert
        var expectedTotal = 160000m; // 75000 + 85000
        Assert.Equal(expectedTotal, totalSalary);
    }
}
