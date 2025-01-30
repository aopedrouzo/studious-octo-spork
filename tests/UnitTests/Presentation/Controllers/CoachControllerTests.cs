using FootballManager.Application.DTOs;
using FootballManager.Application.DTOs.Request;
using FootballManager.Application.Interfaces;
using FootballManager.Domain.Enums;
using FootballManager.Presentation.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FootballClubManagerTests.UnitTests.Presentation.Controllers;

public class CoachControllerTests
{
    private readonly Mock<ICoachService> _coachService;
    private readonly CoachController _sut;

    public CoachControllerTests()
    {
        _coachService = new Mock<ICoachService>();
        _sut = new CoachController(_coachService.Object);
    }

    private static CoachResponseDto CreateTestCoachResponse(
        int id = 1,
        string firstName = "John",
        string lastName = "Doe",
        decimal salary = 75000m)
    {
        return new CoachResponseDto
        {
            Id = id,
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now),
            Salary = salary,
            Email = "john.doe@test.com"
        };
    }

    private static CreateCoachDto CreateTestCoachRequest(
        string firstName = "John",
        string lastName = "Doe",
        decimal salary = 75000m)
    {
        return new CreateCoachDto
        {
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now),
            Salary = salary,
            Email = "john.doe@test.com"
        };
    }

    [Fact]
    public async Task GetCoachById_ExistingCoach_ReturnsOkWithCoach()
    {
        // Arrange
        var coach = CreateTestCoachResponse();
        _coachService.Setup(x => x.GetCoachById(1)).ReturnsAsync(coach);

        // Act
        var result = await _sut.GetCoachById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<CoachResponseDto>(okResult.Value);
        Assert.Equal(coach.FirstName, response.FirstName);
        Assert.Equal(coach.LastName, response.LastName);
    }

    [Fact]
    public async Task GetCoachById_NonExistingCoach_ReturnsNotFound()
    {
        // Arrange
        _coachService.Setup(x => x.GetCoachById(1))
            .ThrowsAsync(new KeyNotFoundException("Coach not found"));

        // Act
        var result = await _sut.GetCoachById(1);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var errorResponse = Assert.IsType<ErrorResponse>(notFoundResult.Value);
        Assert.Equal("Coach not found", errorResponse.Message);
    }

    [Fact]
    public async Task AddCoach_ValidRequest_ReturnsCreatedWithCoach()
    {
        // Arrange
        var request = CreateTestCoachRequest();
        var createdCoach = CreateTestCoachResponse();
        _coachService.Setup(x => x.AddCoach(request)).ReturnsAsync(createdCoach);

        // Act
        var result = await _sut.AddCoach(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(CoachController.GetCoachById), createdResult.ActionName);
        Assert.Equal(createdCoach.Id, createdResult.RouteValues?["id"]);
        var response = Assert.IsType<CoachResponseDto>(createdResult.Value);
        Assert.Equal(request.FirstName, response.FirstName);
        Assert.Equal(request.LastName, response.LastName);
    }

    [Fact]
    public async Task AddCoach_NullRequest_ReturnsBadRequest()
    {
        // Arrange
        var request = CreateTestCoachRequest();
        _coachService.Setup(x => x.AddCoach(request))
            .ThrowsAsync(new ArgumentNullException("request", "Request cannot be null"));

        // Act
        var result = await _sut.AddCoach(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
        Assert.Equal("Request cannot be null (Parameter 'request')", errorResponse.Message);
    }

    [Fact]
    public async Task AddCoach_UnexpectedError_ReturnsInternalServerError()
    {
        // Arrange
        var request = CreateTestCoachRequest();
        _coachService.Setup(x => x.AddCoach(request))
            .ThrowsAsync(new Exception("Unexpected error message"));

        // Act
        var result = await _sut.AddCoach(request);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        var errorResponse = Assert.IsType<ErrorResponse>(statusCodeResult.Value);
        Assert.Equal("An error occurred while processing your request", errorResponse.Message);
        Assert.Equal("Unexpected error message", errorResponse.Details);
    }

    [Fact]
    public async Task TransferCoach_ValidRequest_ReturnsOkWithCoach()
    {
        // Arrange
        var coach = CreateTestCoachResponse();
        _coachService.Setup(x => x.TransferCoach(1, 1)).ReturnsAsync(coach);

        // Act
        var result = await _sut.TransferCoach(1, 1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<CoachResponseDto>(okResult.Value);
        Assert.Equal(coach.FirstName, response.FirstName);
        Assert.Equal(coach.LastName, response.LastName);
    }

    [Fact]
    public async Task TransferCoach_NonExistingCoach_ReturnsNotFound()
    {
        // Arrange
        _coachService.Setup(x => x.TransferCoach(1, 1))
            .ThrowsAsync(new KeyNotFoundException("Coach not found"));

        // Act
        var result = await _sut.TransferCoach(1, 1);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var errorResponse = Assert.IsType<ErrorResponse>(notFoundResult.Value);
        Assert.Equal("Coach not found", errorResponse.Message);
    }

    [Fact]
    public async Task TransferCoach_UnexpectedError_ReturnsInternalServerError()
    {
        // Arrange
        _coachService.Setup(x => x.TransferCoach(1, 1))
            .ThrowsAsync(new Exception("Unexpected error message"));

        // Act
        var result = await _sut.TransferCoach(1, 1);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        var errorResponse = Assert.IsType<ErrorResponse>(statusCodeResult.Value);
        Assert.Equal("An error occurred while processing your request", errorResponse.Message);
        Assert.Equal("Unexpected error message", errorResponse.Details);
    }

    [Fact]
    public async Task ReleaseCoach_ValidRequest_ReturnsOkWithCoach()
    {
        // Arrange
        var coach = CreateTestCoachResponse();
        _coachService.Setup(x => x.ReleaseCoach(1)).ReturnsAsync(coach);

        // Act
        var result = await _sut.ReleaseCoach(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<CoachResponseDto>(okResult.Value);
        Assert.Equal(coach.FirstName, response.FirstName);
        Assert.Equal(coach.LastName, response.LastName);
    }

    [Fact]
    public async Task ReleaseCoach_NonExistingCoach_ReturnsNotFound()
    {
        // Arrange
        _coachService.Setup(x => x.ReleaseCoach(1))
            .ThrowsAsync(new KeyNotFoundException("Coach not found"));

        // Act
        var result = await _sut.ReleaseCoach(1);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var errorResponse = Assert.IsType<ErrorResponse>(notFoundResult.Value);
        Assert.Equal("Coach not found", errorResponse.Message);
    }

    [Fact]
    public async Task ReleaseCoach_UnexpectedError_ReturnsInternalServerError()
    {
        // Arrange
        _coachService.Setup(x => x.ReleaseCoach(1))
            .ThrowsAsync(new Exception("Unexpected error message"));

        // Act
        var result = await _sut.ReleaseCoach(1);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        var errorResponse = Assert.IsType<ErrorResponse>(statusCodeResult.Value);
        Assert.Equal("An error occurred while processing your request", errorResponse.Message);
        Assert.Equal("Unexpected error message", errorResponse.Details);
    }
} 