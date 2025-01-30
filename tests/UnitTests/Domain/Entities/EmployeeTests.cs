using FootballManager.Domain.Entities;
using FootballManager.Domain.Enums;

namespace FootballClubManagerTests.UnitTests.Domain.Entities;

public class EmployeeTests
{
    // Concrete implementation of Employee for testing
    private class TestEmployee : Employee
    {
        public TestEmployee(string firstName, string lastName, DateOnly dateOfBirth, decimal salary, string email)
            : base(firstName, lastName, dateOfBirth, salary, email)
        {
        }
    }

    private static TestEmployee CreateTestEmployee(
        string firstName = "John",
        string lastName = "Doe",
        decimal salary = 50000m,
        string email = "john.doe@test.com")
    {
        return new TestEmployee(
            firstName,
            lastName,
            DateOnly.FromDateTime(DateTime.Now),
            salary,
            email);
    }

    private static Club CreateTestClub(
        string name = "Test FC",
        decimal budget = 1000000m)
    {
        return new Club(name, budget);
    }

    [Fact]
    public void Constructor_ValidParameters_CreatesEmployee()
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";
        var dateOfBirth = DateOnly.FromDateTime(DateTime.Now);
        var salary = 50000m;
        var email = "john.doe@test.com";

        // Act
        var employee = new TestEmployee(firstName, lastName, dateOfBirth, salary, email);

        // Assert
        Assert.Equal(firstName, employee.FirstName);
        Assert.Equal(lastName, employee.LastName);
        Assert.Equal(dateOfBirth, employee.DateOfBirth);
        Assert.Equal(salary, employee.Salary);
        Assert.Equal(email, employee.Email);
        Assert.Null(employee.ClubId);
    }

    [Fact]
    public void JoinClub_ValidClub_SetsClubId()
    {
        // Arrange
        var employee = CreateTestEmployee();
        var club = CreateTestClub();
        var clubId = club.GetType().GetProperty("Id")?.GetValue(club);

        // Act
        employee.JoinClub(club);

        // Assert
        Assert.Equal(clubId, employee.ClubId);
    }

    [Fact]
    public void LeaveClub_WhenCalled_ClearsClubId()
    {
        // Arrange
        var employee = CreateTestEmployee();
        var club = CreateTestClub();
        employee.JoinClub(club);

        // Act
        employee.LeaveClub();

        // Assert
        Assert.Null(employee.ClubId);
    }

   
} 