using FootballManager.Application.DTOs;
using FootballManager.Application.DTOs.Request;
using FootballManager.Application.Interfaces;
using FootballManager.Application.Services;
using FootballManager.Domain.Entities;
using Moq;

namespace FootballClubManagerTests.UnitTests.Services;

public class CoachServiceTests
{
    private readonly Mock<ICoachRepository> _coachRepository;
    private readonly Mock<IClubRepository> _clubRepository;
    private readonly CoachService _sut;

    public CoachServiceTests()
    {
        _coachRepository = new Mock<ICoachRepository>();
        _clubRepository = new Mock<IClubRepository>();
        _sut = new CoachService(_coachRepository.Object, _clubRepository.Object);
    }

    [Fact]
    public async Task GetCoachById_ExistingCoach_ReturnsCoachDto()
    {
        // Arrange
        var coach = new Coach("John", "Doe", DateOnly.FromDateTime(DateTime.Now), 50000m, "john.doe@test.com");
        _coachRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(coach);

        // Act
        var result = await _sut.GetCoachById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(coach.FirstName, result.FirstName);
        Assert.Equal(coach.LastName, result.LastName);
    }

    [Fact]
    public async Task AddCoach_ValidRequest_ReturnsNewCoachDto()
    {
        // Arrange
        var request = new CreateCoachDto
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now),
            Salary = 50000m,
            Email = "john.doe@test.com"
        };

        var coach = new Coach(request.FirstName, request.LastName, request.DateOfBirth, request.Salary, request.Email);

        _coachRepository.Setup(x => x.AddAsync(It.IsAny<Coach>())).ReturnsAsync(coach);

        // Act
        var result = await _sut.AddCoach(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.FirstName, result.FirstName);
        Assert.Equal(request.LastName, result.LastName);
        Assert.Equal(request.Salary, result.Salary);
        Assert.Equal(request.Email, result.Email);
        _coachRepository.Verify(x => x.AddAsync(It.IsAny<Coach>()), Times.Once);
    }

    [Fact]
    public async Task ReleaseCoach_ExistingCoach_ReturnsUpdatedCoachDto()
    {
        // Arrange
        var coach = new Coach("John", "Doe", DateOnly.FromDateTime(DateTime.Now), 50000m, "john.doe@test.com");
        _coachRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(coach);
        _coachRepository.Setup(x => x.UpdateAsync(It.IsAny<Coach>())).Returns(Task.FromResult(coach));

        // Act
        var result = await _sut.ReleaseCoach(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(coach.FirstName, result.FirstName);
        Assert.Equal(coach.LastName, result.LastName);
        _coachRepository.Verify(x => x.UpdateAsync(It.IsAny<Coach>()), Times.Once);
    }

    [Fact]
    public async Task TransferCoach_ExistingCoachAndClub_ReturnsUpdatedCoachDto()
    {
        // Arrange
        var coach = new Coach("John", "Doe", DateOnly.FromDateTime(DateTime.Now), 50000m, "john.doe@test.com");
        var club = new Club("Test FC", 1000000m);
        
        _coachRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(coach);
        _clubRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(club);
        _coachRepository.Setup(x => x.UpdateAsync(It.IsAny<Coach>())).Returns(Task.FromResult(coach));

        // Act
        var result = await _sut.TransferCoach(1, 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(coach.FirstName, result.FirstName);
        Assert.Equal(coach.LastName, result.LastName);
        _coachRepository.Verify(x => x.UpdateAsync(It.IsAny<Coach>()), Times.Once);
    }

    [Fact]
    public async Task GetCoachById_NonExistingCoach_ThrowsKeyNotFoundException()
    {
        // Arrange
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        _coachRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Coach?)null);
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetCoachById(1));
    }

    [Fact]
    public async Task ReleaseCoach_NonExistingCoach_ThrowsKeyNotFoundException()
    {
        // Arrange
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        _coachRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Coach?)null);
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.ReleaseCoach(1));
    }

    [Fact]
    public async Task TransferCoach_NonExistingCoach_ThrowsKeyNotFoundException()
    {
        // Arrange
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        _coachRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Coach?)null);
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        var club = new Club("Test FC", 1000000m);
        _clubRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(club);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.TransferCoach(1, 1));
    }

    [Fact]
    public async Task TransferCoach_NonExistingClub_ThrowsKeyNotFoundException()
    {
        // Arrange
        var coach = new Coach("John", "Doe", DateOnly.FromDateTime(DateTime.Now), 50000m, "john.doe@test.com");
        _coachRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(coach);
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        _clubRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Club?)null);
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.TransferCoach(1, 1));
    }
} 