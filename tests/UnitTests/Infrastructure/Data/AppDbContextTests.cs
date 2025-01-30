using Microsoft.EntityFrameworkCore;
using FootballManager.Infrastructure.Data;
using FootballManager.Domain.Entities;
using FootballManager.Domain.Enums;
using Xunit;

namespace FootballClubManagerTests.UnitTests.Infrastructure.Data;

public class AppDbContextTests
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

    [Fact]
    public async Task CanAddAndRetrieveClub()
    {
        // Arrange
        using var context = CreateContext();
        var club = new Club("Test FC", 1000000M);

        // Act
        context.Clubs.Add(club);
        await context.SaveChangesAsync();

        // Assert
        var savedClub = await context.Clubs.FirstOrDefaultAsync(c => c.Name == "Test FC");
        Assert.NotNull(savedClub);
        Assert.Equal(club.Name, savedClub.Name);
        Assert.Equal(club.Budget, savedClub.Budget);
    }

    [Fact]
    public async Task ClubPlayerRelationshipWorks()
    {
        // Arrange
        using var context = CreateContext();
        var club = new Club("Test FC", 1000000M);
        
        var player = new Player(
            "John",
            "Doe",
            DateOnly.FromDateTime(DateTime.Now),
            Position.Forward,
            50000M,
            "john.doe@testfc.com"
        );

        // Act
        club.Players.Add(player);
        context.Clubs.Add(club);
        await context.SaveChangesAsync();

        // Assert
        var savedClub = await context.Clubs
            .Include(c => c.Players)
            .FirstOrDefaultAsync(c => c.Name == "Test FC");
            
        Assert.NotNull(savedClub);
        Assert.Single(savedClub.Players);
        Assert.Equal(player.FirstName, savedClub.Players.First().FirstName);
    }

    [Fact]
    public async Task ClubCoachRelationshipWorks()
    {
        // Arrange
        using var context = CreateContext();
        var club = new Club("Test FC", 1000000M);
        
        var coach = new Coach(
            "Jane",
            "Smith",
            DateOnly.FromDateTime(DateTime.Now),
            75000M,
            "jane.smith@testfc.com"
        );

        // Act
        club.Coaches.Add(coach);
        context.Clubs.Add(club);
        await context.SaveChangesAsync();

        // Assert
        var savedClub = await context.Clubs
            .Include(c => c.Coaches)
            .FirstOrDefaultAsync(c => c.Name == "Test FC");
            
        Assert.NotNull(savedClub);
        Assert.Single(savedClub.Coaches);
        Assert.Equal(coach.FirstName, savedClub.Coaches.First().FirstName);
    }

    [Fact]
    public async Task SeedDataIsCreated()
    {
        // Arrange
        using var context = CreateContext();

        // Act & Assert
        Assert.Equal(4, await context.Clubs.CountAsync());
        Assert.Equal(3, await context.Coaches.CountAsync());
        Assert.Equal(12, await context.Players.CountAsync());
        Assert.Equal(1, await context.Users.CountAsync());

        // Verify specific seeded data
        var manutd = await context.Clubs.FindAsync(1);
        Assert.NotNull(manutd);
        Assert.Equal("Manchester United", manutd.Name);
        Assert.Equal(100000000M, manutd.Budget);

        var klopp = await context.Coaches.FindAsync(2);
        Assert.NotNull(klopp);
        Assert.Equal("Jurgen", klopp.FirstName);
        Assert.Equal("Klopp", klopp.LastName);

        var saka = await context.Players.FindAsync(12);
        Assert.NotNull(saka);
        Assert.Equal("Bukayo", saka.FirstName);
        Assert.Equal("Saka", saka.LastName);
        Assert.Equal(Position.Forward, saka.Position);
    }

} 