using FootballManager.Application.DTOs;
using FootballManager.Application.DTOs.Request;
using FootballManager.Application.Interfaces;
using FootballManager.Domain.Enums;
using FootballManager.Presentation.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FootballClubManagerTests.UnitTests.Presentation.Controllers;

public class PlayerControllerTests
{
    private readonly Mock<IPlayerService> _playerService;
    private readonly PlayerController _sut;

    public PlayerControllerTests()
    {
        _playerService = new Mock<IPlayerService>();
        _sut = new PlayerController(_playerService.Object);
    }

    private static PlayerResponseDto CreateTestPlayerResponse(
        int id = 1,
        string firstName = "John",
        string lastName = "Doe",
        Position position = Position.Forward,
        decimal salary = 50000m)
    {
        return new PlayerResponseDto
        {
            Id = id,
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now),
            Position = position,
            Salary = salary,
            Email = "john.doe@test.com"
        };
    }

    private static CreatePlayerDto CreateTestPlayerRequest(
        string firstName = "John",
        string lastName = "Doe",
        Position position = Position.Forward,
        decimal salary = 50000m)
    {
        return new CreatePlayerDto
        {
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now),
            Position = position,
            Salary = salary,
            Email = "john.doe@test.com"
        };
    }

    [Fact]
    public async Task GetPlayerById_ExistingPlayer_ReturnsOkWithPlayer()
    {
        // Arrange
        var player = CreateTestPlayerResponse();
        _playerService.Setup(x => x.GetPlayerById(1)).ReturnsAsync(player);

        // Act
        var result = await _sut.GetPlayerById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<PlayerResponseDto>(okResult.Value);
        Assert.Equal(player.FirstName, response.FirstName);
        Assert.Equal(player.LastName, response.LastName);
        Assert.Equal(player.Position, response.Position);
    }

    [Fact]
    public async Task GetPlayerById_NonExistingPlayer_ReturnsNotFound()
    {
        // Arrange
        _playerService.Setup(x => x.GetPlayerById(1))
            .ThrowsAsync(new KeyNotFoundException("Player not found"));

        // Act
        var result = await _sut.GetPlayerById(1);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var errorResponse = Assert.IsType<ErrorResponse>(notFoundResult.Value);
        Assert.Equal("Player not found", errorResponse.Message);
    }

    [Fact]
    public async Task AddPlayer_NonExistingClub_ReturnsNotFound()
    {
        // Arrange
        var request = CreateTestPlayerRequest();
        _playerService.Setup(x => x.AddPlayer(request))
            .ThrowsAsync(new KeyNotFoundException("Club not found"));

        // Act
        var result = await _sut.AddPlayer(request);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var errorResponse = Assert.IsType<ErrorResponse>(notFoundResult.Value);
        Assert.Equal("Club not found", errorResponse.Message);
    }

    [Fact]
    public async Task GetPlayerById_UnexpectedError_ReturnsInternalServerError()
    {
        // Arrange
        _playerService.Setup(x => x.GetPlayerById(1))
            .ThrowsAsync(new Exception("Unexpected error message"));

        // Act
        var result = await _sut.GetPlayerById(1);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        var errorResponse = Assert.IsType<ErrorResponse>(statusCodeResult.Value);
        Assert.Equal("An error occurred while processing your request", errorResponse.Message);
        Assert.Equal("Unexpected error message", errorResponse.Details);
    }

    [Fact]
    public async Task AddPlayer_ValidRequest_ReturnsCreatedWithPlayer()
    {
        // Arrange
        var request = CreateTestPlayerRequest();
        var createdPlayer = CreateTestPlayerResponse();
        _playerService.Setup(x => x.AddPlayer(request)).ReturnsAsync(createdPlayer);

        // Act
        var result = await _sut.AddPlayer(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(PlayerController.GetPlayerById), createdResult.ActionName);
        Assert.Equal(createdPlayer.Id, createdResult.RouteValues?["id"]);
        var response = Assert.IsType<PlayerResponseDto>(createdResult.Value);
        Assert.Equal(request.FirstName, response.FirstName);
        Assert.Equal(request.LastName, response.LastName);
        Assert.Equal(request.Position, response.Position);
    }

    [Fact]
    public async Task AddPlayer_UnexpectedError_ReturnsInternalServerError()
    {
        // Arrange
        var request = CreateTestPlayerRequest();
        _playerService.Setup(x => x.AddPlayer(request))
            .ThrowsAsync(new Exception("Unexpected error message"));

        // Act
        var result = await _sut.AddPlayer(request);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        var errorResponse = Assert.IsType<ErrorResponse>(statusCodeResult.Value);
        Assert.Equal("An error occurred while processing your request", errorResponse.Message);
        Assert.Equal("Unexpected error message", errorResponse.Details);
    }

    [Fact]
    public async Task TransferPlayer_ValidRequest_ReturnsOkWithPlayer()
    {
        // Arrange
        var player = CreateTestPlayerResponse();
        _playerService.Setup(x => x.TransferPlayer(1, 1)).ReturnsAsync(player);

        // Act
        var result = await _sut.TransferPlayer(1, 1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<PlayerResponseDto>(okResult.Value);
        Assert.Equal(player.FirstName, response.FirstName);
        Assert.Equal(player.LastName, response.LastName);
        Assert.Equal(player.Position, response.Position);
    }

    [Fact]
    public async Task TransferPlayer_NonExistingPlayer_ReturnsNotFound()
    {
        // Arrange
        _playerService.Setup(x => x.TransferPlayer(1, 1))
            .ThrowsAsync(new KeyNotFoundException("Player not found"));

        // Act
        var result = await _sut.TransferPlayer(1, 1);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var errorResponse = Assert.IsType<ErrorResponse>(notFoundResult.Value);
        Assert.Equal("Player not found", errorResponse.Message);
    }

    [Fact]
    public async Task TransferPlayer_UnexpectedError_ReturnsInternalServerError()
    {
        // Arrange
        _playerService.Setup(x => x.TransferPlayer(1, 1))
            .ThrowsAsync(new Exception("Unexpected error message"));

        // Act
        var result = await _sut.TransferPlayer(1, 1);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        var errorResponse = Assert.IsType<ErrorResponse>(statusCodeResult.Value);
        Assert.Equal("An error occurred while processing your request", errorResponse.Message);
        Assert.Equal("Unexpected error message", errorResponse.Details);
    }

    [Fact]
    public async Task ReleasePlayer_ValidRequest_ReturnsOkWithPlayer()
    {
        // Arrange
        var player = CreateTestPlayerResponse();
        _playerService.Setup(x => x.ReleasePlayer(1)).ReturnsAsync(player);

        // Act
        var result = await _sut.ReleasePlayer(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<PlayerResponseDto>(okResult.Value);
        Assert.Equal(player.FirstName, response.FirstName);
        Assert.Equal(player.LastName, response.LastName);
        Assert.Equal(player.Position, response.Position);
    }

    [Fact]
    public async Task ReleasePlayer_NonExistingPlayer_ReturnsNotFound()
    {
        // Arrange
        _playerService.Setup(x => x.ReleasePlayer(1))
            .ThrowsAsync(new KeyNotFoundException("Player not found"));

        // Act
        var result = await _sut.ReleasePlayer(1);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var errorResponse = Assert.IsType<ErrorResponse>(notFoundResult.Value);
        Assert.Equal("Player not found", errorResponse.Message);
    }

    [Fact]
    public async Task ReleasePlayer_UnexpectedError_ReturnsInternalServerError()
    {
        // Arrange
        _playerService.Setup(x => x.ReleasePlayer(1))
            .ThrowsAsync(new Exception("Unexpected error message"));

        // Act
        var result = await _sut.ReleasePlayer(1);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        var errorResponse = Assert.IsType<ErrorResponse>(statusCodeResult.Value);
        Assert.Equal("An error occurred while processing your request", errorResponse.Message);
        Assert.Equal("Unexpected error message", errorResponse.Details);
    }
}