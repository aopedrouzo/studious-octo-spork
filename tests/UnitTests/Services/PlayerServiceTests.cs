using System.Reflection;
using FootballManager.Application.DTOs;
using FootballManager.Application.DTOs.Request;
using FootballManager.Application.Interfaces;
using FootballManager.Application.Services;
using FootballManager.Domain.Entities;
using FootballManager.Domain.Enums;
using Moq;

namespace FootballClubManagerTests.UnitTests.Services;

public class PlayerServiceTests
{
    private readonly Mock<IPlayerRepository> _playerRepository;
    private readonly Mock<IClubRepository> _clubRepository;
    private readonly Mock<INotificationService> _notificationService;
    private readonly PlayerService _sut;

    public PlayerServiceTests()
    {
        _playerRepository = new Mock<IPlayerRepository>();
        _clubRepository = new Mock<IClubRepository>();
        _notificationService = new Mock<INotificationService>();
        _sut = new PlayerService(_playerRepository.Object, _clubRepository.Object, _notificationService.Object);
    }

    [Fact]
    public async Task GetPlayerById_ExistingPlayer_ReturnsPlayerDto()
    {
        // Arrange
        var player = new Player("John", "Doe", DateOnly.FromDateTime(DateTime.Now), Position.Forward, 50000m, "john.doe@test.com");
        _playerRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(player);

        // Act
        var result = await _sut.GetPlayerById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(player.FirstName, result.FirstName);
        Assert.Equal(player.LastName, result.LastName);
    }

    [Fact]
    public async Task ReleasePlayer_ExistingPlayer_ReturnsUpdatedPlayerDto()
    {
        // Arrange
        var player = new Player("John", "Doe", DateOnly.FromDateTime(DateTime.Now), Position.Forward, 50000m, "john.doe@test.com");
        _playerRepository.Setup(x => x.GetByIdAsync(1)).Returns(Task.FromResult(player));
        _playerRepository.Setup(x => x.UpdateAsync(player)).Returns(value: Task.FromResult(player));

        // Act
        var result = await _sut.ReleasePlayer(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(player.FirstName, result.FirstName);
        Assert.Equal(player.LastName, result.LastName);
        _playerRepository.Verify(x => x.UpdateAsync(It.IsAny<Player>()), Times.Once);
        _notificationService.Verify(x => x.SendNotificationAsync(It.IsAny<NotificationMessage>()), Times.Once);
    }

    [Fact]
    public async Task TransferPlayer_ExistingPlayerAndClub_ReturnsUpdatedPlayerDto()
    {
        // Arrange
        var player = new Player("John", "Doe", DateOnly.FromDateTime(DateTime.Now), Position.Forward, 50000m, "john.doe@test.com");
        var club = new Club("Test FC", 1000000m);

        _playerRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(player);
        _clubRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(club);
        _playerRepository.Setup(x => x.UpdateAsync(player)).Returns(value: Task.FromResult(player));

        // Act
        var result = await _sut.TransferPlayer(1, 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(player.FirstName, result.FirstName);
        Assert.Equal(player.LastName, result.LastName);
        _playerRepository.Verify(x => x.UpdateAsync(It.IsAny<Player>()), Times.Once);
        _notificationService.Verify(x => x.SendNotificationAsync(It.IsAny<NotificationMessage>()), Times.Once);
    }

    [Fact]
    public async Task AddPlayer_ValidRequest_ReturnsNewPlayerDto()
    {
        // Arrange
        var request = new CreatePlayerDto
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now),
            Position = Position.Forward,
            Salary = 50000m,
            Email = "john.doe@test.com"
        };

        var player = new Player(request.FirstName, request.LastName, request.DateOfBirth, request.Position, request.Salary, request.Email);

        _playerRepository.Setup(x => x.AddAsync(It.IsAny<Player>())).ReturnsAsync(player);

        // Act
        var result = await _sut.AddPlayer(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.FirstName, result.FirstName);
        Assert.Equal(request.LastName, result.LastName);
        Assert.Equal(request.Position, result.Position);
        Assert.Equal(request.Salary, result.Salary);
        Assert.Equal(request.Email, result.Email);
        _playerRepository.Verify(x => x.AddAsync(It.IsAny<Player>()), Times.Once);
    }

    [Fact]
    public async Task GetPlayerById_NonExistingPlayer_ThrowsKeyNotFoundException()
    {
        // Arrange
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        _playerRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Player?)null);
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetPlayerById(1));
    }

    [Fact]
    public async Task ReleasePlayer_NonExistingPlayer_ThrowsKeyNotFoundException()
    {
        // Arrange
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        _playerRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Player?)null);
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.ReleasePlayer(1));
    }

    [Fact]
    public async Task TransferPlayer_NonExistingPlayer_ThrowsKeyNotFoundException()
    {
        // Arrange
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        _playerRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Player?)null);
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        var club = new Club("Test FC", 1000000m);
        _clubRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(club);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.TransferPlayer(1, 1));
    }

    [Fact]
    public async Task TransferPlayer_NonExistingClub_ThrowsKeyNotFoundException()
    {
        // Arrange
        var player = new Player("John", "Doe", DateOnly.FromDateTime(DateTime.Now), Position.Forward, 50000m, "john.doe@test.com");
        _playerRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(player);
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        _clubRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Club?)null);
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.TransferPlayer(1, 1));
    }

    [Fact]
    public async Task TransferPlayer_UpdatesClubBudgets()
    {
        // Arrange
        var player = new Player("John", "Doe", DateOnly.FromDateTime(DateTime.Now), Position.Forward, 50000m, "john.doe@test.com");
        var originClub = new Club("Origin FC", 100000m);
        originClub.GetType().GetProperty("Id")?.SetValue(originClub, 1);

        var destinationClub = new Club("Destination FC", 200000m);
        destinationClub.GetType().GetProperty("Id")?.SetValue(destinationClub, 2);

        // Setup player with original club
        player.GetType().GetProperty("ClubId")?.SetValue(player, 1);

        _playerRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(player);
        _clubRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(originClub);
        _clubRepository.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(destinationClub);
        _playerRepository.Setup(x => x.UpdateAsync(It.IsAny<Player>())).Returns(Task.CompletedTask);
        _clubRepository.Setup(x => x.UpdateAsync(It.IsAny<Club>())).Returns(Task.CompletedTask);

        // Act
        await _sut.TransferPlayer(1, 2);

        // Assert
        _clubRepository.Verify(x => x.UpdateAsync(It.Is<Club>(c => c.Id == originClub.Id)), Times.Once);
        _clubRepository.Verify(x => x.UpdateAsync(It.Is<Club>(c => c.Id == destinationClub.Id)), Times.Once);
        Assert.Equal(150000m, originClub.Budget); // Original budget + player salary
        Assert.Equal(150000m, destinationClub.Budget); // Original budget - player salary
    }

    [Fact]
    public async Task ReleasePlayer_UpdatesClubBudget()
    {
        // Arrange
        var player = new Player("John", "Doe", DateOnly.FromDateTime(DateTime.Now), Position.Forward, 50000m, "john.doe@test.com");
        var club = new Club("Test FC", 100000m);

        // Setup player with club
        player.GetType().GetProperty("ClubId")?.SetValue(player, club.Id);

        _playerRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(player);
        _clubRepository.Setup(x => x.GetByIdAsync(club.Id)).ReturnsAsync(club);
        _playerRepository.Setup(x => x.UpdateAsync(It.IsAny<Player>())).Returns(Task.CompletedTask);
        _clubRepository.Setup(x => x.UpdateAsync(It.IsAny<Club>())).Returns(Task.CompletedTask);

        // Act
        await _sut.ReleasePlayer(1);

        // Assert
        _clubRepository.Verify(x => x.UpdateAsync(It.Is<Club>(c => c.Id == club.Id)), Times.Once);
        Assert.Equal(150000m, club.Budget); // Original budget + player salary
    }
}