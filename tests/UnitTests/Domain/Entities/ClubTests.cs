using FootballManager.Domain.Entities;
using FootballManager.Domain.Enums;

namespace FootballClubManagerTests.UnitTests.Domain.Entities;

public class ClubTests
{
    private static Club CreateTestClub(
        string name = "Test FC",
        decimal budget = 1000000m)
    {
        return new Club(name, budget);
    }

    private static Player CreateTestPlayer(
        string firstName = "John",
        string lastName = "Doe",
        decimal salary = 50000m,
        int? clubId = null)
    {
        var player = new Player(
            firstName,
            lastName,
            DateOnly.FromDateTime(DateTime.Now),
            Position.Forward,
            salary,
            "john.doe@test.com");

        if (clubId.HasValue)
        {
            player.GetType().GetProperty("ClubId")?.SetValue(player, clubId);
        }

        return player;
    }

    private static Coach CreateTestCoach(
        string firstName = "Jane",
        string lastName = "Smith",
        decimal salary = 75000m,
        int? clubId = null)
    {
        var coach = new Coach(
            firstName,
            lastName,
            DateOnly.FromDateTime(DateTime.Now),
            salary,
            "jane.smith@test.com");

        if (clubId.HasValue)
        {
            coach.GetType().GetProperty("ClubId")?.SetValue(coach, clubId);
        }

        return coach;
    }

    [Fact]
    public void Constructor_ValidParameters_CreatesClub()
    {
        // Arrange & Act
        var club = CreateTestClub();

        // Assert
        Assert.NotNull(club);
        Assert.Equal("Test FC", club.Name);
        Assert.Equal(1000000m, club.Budget);
        Assert.Empty(club.Players);
        Assert.Empty(club.Coaches);
    }

    [Fact]
    public void HandleTransfer_ValidEmployee_DeductsSalaryFromBudget()
    {
        // Arrange
        var club = CreateTestClub(budget: 100000m);
        var player = CreateTestPlayer(salary: 50000m);

        // Act
        club.HandleTransfer(player);

        // Assert
        Assert.Equal(50000m, club.Budget);
    }

    [Fact]
    public void HandleTransfer_EmployeeWithExistingClub_ThrowsInvalidOperationException()
    {
        // Arrange
        var club = CreateTestClub();
        var player = CreateTestPlayer(clubId: 2);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => club.HandleTransfer(player));
        Assert.Contains("already assigned to another club", exception.Message);
    }

    [Fact]
    public void HandleTransfer_InsufficientBudget_ThrowsInvalidOperationException()
    {
        // Arrange
        var club = CreateTestClub(budget: 40000m);
        var player = CreateTestPlayer(salary: 50000m);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => club.HandleTransfer(player));
        Assert.Contains("Insufficient budget", exception.Message);
    }

    [Fact]
    public void HandleTransfer_InvalidSalary_ThrowsInvalidOperationException()
    {
        // Arrange
        var club = CreateTestClub();
        var player = CreateTestPlayer(salary: 0m);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => club.HandleTransfer(player));
        Assert.Contains("must have a valid salary", exception.Message);
    }

    [Fact]
    public void ModifyBudget_ValidAmount_UpdatesBudget()
    {
        // Arrange
        var club = CreateTestClub(budget: 100000m);
        var totalSalary = 50000m;

        // Act
        club.ModifyBudget(50000m, totalSalary);

        // Assert
        Assert.Equal(150000m, club.Budget);
    }

    [Fact]
    public void ModifyBudget_AmountBelowTotalSalary_ThrowsInvalidOperationException()
    {
        // Arrange
        var club = CreateTestClub(budget: 100000m);
        var totalSalary = 80000m;

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(
            () => club.ModifyBudget(-30000m, totalSalary));
        Assert.Contains("Budget cannot be reduced below the total salary", exception.Message);
    }

    [Fact]
    public void HandleRelease_ValidEmployee_AddsSalaryToBudget()
    {
        // Arrange
        var club = CreateTestClub(budget: 50000m);
        var player = CreateTestPlayer(salary: 30000m);
        player.GetType().GetProperty("ClubId")?.SetValue(player, club.GetType().GetProperty("Id")?.GetValue(club));

        // Act
        club.HandleRelease(player);

        // Assert
        Assert.Equal(80000m, club.Budget);
    }

    [Fact]
    public void HandleRelease_EmployeeFromDifferentClub_ThrowsInvalidOperationException()
    {
        // Arrange
        var club = CreateTestClub();
        var player = CreateTestPlayer(clubId: 2);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => club.HandleRelease(player));
        Assert.Contains("not assigned to this club", exception.Message);
    }
}
