using FootballManager.Application.Common;
using FootballManager.Application.DTOs;
using FootballManager.Application.DTOs.Request;
using FootballManager.Application.DTOs.Response;
using FootballManager.Application.Interfaces;
using FootballManager.Domain.Enums;
using FootballManager.Presentation.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FootballClubManagerTests.UnitTests.Presentation.Controllers;

public class ClubControllerTests
{
    private readonly Mock<IClubService> _clubService;
    private readonly ClubController _sut;

    public ClubControllerTests()
    {
        _clubService = new Mock<IClubService>();
        _sut = new ClubController(_clubService.Object);
    }

    private static ClubResponseDto CreateTestClubResponse(
        int id = 1,
        string name = "Test FC",
        decimal budget = 1000000m)
    {
        return new ClubResponseDto
        {
            Id = id,
            Name = name,
            Budget = budget,
        };
    }

    private static CreateClubDto CreateTestClubRequest(
        string name = "Test FC",
        decimal budget = 1000000m)
    {
        return new CreateClubDto
        {
            Name = name,
            Budget = budget
        };
    }

    private static CreatePlayerDto CreateTestPlayerDto()
    {
        return new CreatePlayerDto
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now),
            Position = Position.Forward,
            Salary = 50000m,
            Email = "john.doe@test.com"
        };
    }

    [Fact]
    public async Task GetAllClubs_ReturnsOkWithClubs()
    {
        // Arrange
        var clubs = new ClubListResponseDto
        {
            Clubs = new[] {
                new ClubListItemDto { Id = 1, Name = "Test FC" },
                new ClubListItemDto { Id = 2, Name = "Another FC" }
            }
        };
        _clubService.Setup(x => x.GetAllClubs()).ReturnsAsync(clubs);

        // Act
        var result = await _sut.GetAllClubs();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ClubListResponseDto>(okResult.Value);
        Assert.Equal(2, response.Clubs.Count());
    }

    [Fact]
    public async Task GetAllClubs_ServiceThrowsException_ReturnsBadRequest()
    {
        // Arrange
        _clubService.Setup(x => x.GetAllClubs())
            .ThrowsAsync(new InvalidOperationException("Test error"));

        // Act
        var result = await _sut.GetAllClubs();

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
        Assert.Equal("Test error", errorResponse.Message);
    }

    [Fact]
    public async Task GetClubById_ExistingClub_ReturnsOkWithClub()
    {
        // Arrange
        var club = CreateTestClubResponse();
        _clubService.Setup(x => x.GetClubById(1)).ReturnsAsync(club);

        // Act
        var result = await _sut.GetClubById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ClubResponseDto>(okResult.Value);
        Assert.Equal(club.Name, response.Name);
        Assert.Equal(club.Budget, response.Budget);
    }

    [Fact]
    public async Task GetClubById_InvalidOperation_ReturnsBadRequest()
    {
        // Arrange
        _clubService.Setup(x => x.GetClubById(1))
            .ThrowsAsync(new InvalidOperationException("Invalid operation message"));

        // Act
        var result = await _sut.GetClubById(1);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
        Assert.Equal("Invalid operation message", errorResponse.Message);
    }

    [Fact]
    public async Task GetClubById_UnexpectedError_ReturnsInternalServerError()
    {
        // Arrange
        _clubService.Setup(x => x.GetClubById(1))
            .ThrowsAsync(new Exception("Unexpected error message"));

        // Act
        var result = await _sut.GetClubById(1);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        var errorResponse = Assert.IsType<ErrorResponse>(statusCodeResult.Value);
        Assert.Equal("An error occurred while processing your request", errorResponse.Message);
        Assert.Equal("Unexpected error message", errorResponse.Details);
    }

    [Fact]
    public async Task CreateClub_ValidRequest_ReturnsCreatedWithClub()
    {
        // Arrange
        var request = CreateTestClubRequest();
        var createdClub = CreateTestClubResponse();
        _clubService.Setup(x => x.CreateClub(request)).ReturnsAsync(createdClub);

        // Act
        var result = await _sut.CreateClub(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(ClubController.GetClubById), createdResult.ActionName);
        Assert.Equal(createdClub.Id, createdResult?.RouteValues?["id"]);
        var response = Assert.IsType<ClubResponseDto>(createdResult?.Value);
        Assert.Equal(request.Name, response.Name);
        Assert.Equal(request.Budget, response.Budget);
    }

    [Fact]
    public async Task CreateClub_InvalidOperation_ReturnsBadRequest()
    {
        // Arrange
        var request = CreateTestClubRequest();
        _clubService.Setup(x => x.CreateClub(request))
            .ThrowsAsync(new InvalidOperationException("Invalid operation message"));

        // Act
        var result = await _sut.CreateClub(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
        Assert.Equal("Invalid operation message", errorResponse.Message);
    }

    [Fact]
    public async Task CreateClub_UnexpectedError_ReturnsInternalServerError()
    {
        // Arrange
        var request = CreateTestClubRequest();
        _clubService.Setup(x => x.CreateClub(request))
            .ThrowsAsync(new Exception("Unexpected error message"));

        // Act
        var result = await _sut.CreateClub(request);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        var errorResponse = Assert.IsType<ErrorResponse>(statusCodeResult.Value);
        Assert.Equal("An error occurred while processing your request", errorResponse.Message);
        Assert.Equal("Unexpected error message", errorResponse.Details);
    }

    [Fact]
    public async Task AddPlayersToClub_ValidRequest_ReturnsCreatedWithClub()
    {
        // Arrange
        var request = new AddPlayersToClubRequest
        {
            Players = new[]
            {
                new CreatePlayerDto
                {
                    FirstName = "John",
                    LastName = "Doe",
                    DateOfBirth = DateOnly.FromDateTime(DateTime.Now),
                    Position = Position.Forward,
                    Salary = 50000m,
                    Email = "john.doe@test.com"
                }
            }
        };
        var updatedClub = CreateTestClubResponse();
        _clubService.Setup(x => x.AddPlayersToClub(1, request)).ReturnsAsync(updatedClub);

        // Act
        var result = await _sut.AddPlayersToClub(1, request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(ClubController.GetClubPlayers), createdResult.ActionName);
        var response = Assert.IsType<ClubResponseDto>(createdResult.Value);
        Assert.Equal(updatedClub.Name, response.Name);
    }

    [Fact]
    public async Task AddPlayersToClub_InvalidOperation_ReturnsBadRequest()
    {
        // Arrange
        var request = new AddPlayersToClubRequest
        {
            Players = new[] { CreateTestPlayerDto() }
        };
        _clubService.Setup(x => x.AddPlayersToClub(1, request))
            .ThrowsAsync(new InvalidOperationException("Invalid operation message"));

        // Act
        var result = await _sut.AddPlayersToClub(1, request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
        Assert.Equal("Invalid operation message", errorResponse.Message);
    }

    [Fact]
    public async Task AddPlayersToClub_UnexpectedError_ReturnsInternalServerError()
    {
        // Arrange
        var request = new AddPlayersToClubRequest
        {
            Players = new[] { CreateTestPlayerDto() }
        };
        _clubService.Setup(x => x.AddPlayersToClub(1, request))
            .ThrowsAsync(new Exception("Unexpected error message"));

        // Act
        var result = await _sut.AddPlayersToClub(1, request);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        var errorResponse = Assert.IsType<ErrorResponse>(statusCodeResult.Value);
        Assert.Equal("An error occurred while processing your request", errorResponse.Message);
        Assert.Equal("Unexpected error message", errorResponse.Details);
    }

    [Fact]
    public async Task AddCoachToClub_ValidRequest_ReturnsCreatedWithResponse()
    {
        // Arrange
        var request = new CreateCoachDto
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now),
            Salary = 75000m,
            Email = "john.doe@test.com"
        };
        var response = new AddCoachToClubResponseDto
        {
            Club = CreateTestClubResponse(),
            Coach = new CoachResponseDto
            {
                Id = 1,
                FirstName = request.FirstName,
                LastName = request.LastName
            }
        };
        _clubService.Setup(x => x.AddCoachToClub(1, request)).ReturnsAsync(response);

        // Act
        var result = await _sut.AddCoachToClub(1, request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal("GetCoachById", createdResult.ActionName);
        Assert.Equal("Coach", createdResult.ControllerName);
        var responseDto = Assert.IsType<AddCoachToClubResponseDto>(createdResult.Value);
        Assert.Equal(response.Coach.FirstName, responseDto.Coach.FirstName);
    }

    [Fact]
    public async Task AdjustClubBudget_ValidRequest_ReturnsOkWithUpdatedClub()
    {
        // Arrange
        var request = new UpdateClubBudgetRequest { Amount = 500000m };
        var updatedClub = CreateTestClubResponse(budget: 1500000m);
        _clubService.Setup(x => x.AdjustClubBudget(1, request)).ReturnsAsync(updatedClub);

        // Act
        var result = await _sut.AdjustClubBudget(1, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ClubResponseDto>(okResult.Value);
        Assert.Equal(updatedClub.Budget, response.Budget);
    }

    [Fact]
    public async Task AdjustClubBudget_InvalidOperation_ReturnsBadRequest()
    {
        // Arrange
        var request = new UpdateClubBudgetRequest { Amount = 500000m };
        _clubService.Setup(x => x.AdjustClubBudget(1, request))
            .ThrowsAsync(new InvalidOperationException("Invalid operation message"));

        // Act
        var result = await _sut.AdjustClubBudget(1, request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
        Assert.Equal("Invalid operation message", errorResponse.Message);
    }

    [Fact]
    public async Task AdjustClubBudget_UnexpectedError_ReturnsInternalServerError()
    {
        // Arrange
        var request = new UpdateClubBudgetRequest { Amount = 500000m };
        _clubService.Setup(x => x.AdjustClubBudget(1, request))
            .ThrowsAsync(new Exception("Unexpected error message"));

        // Act
        var result = await _sut.AdjustClubBudget(1, request);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        var errorResponse = Assert.IsType<ErrorResponse>(statusCodeResult.Value);
        Assert.Equal("An error occurred while processing your request", errorResponse.Message);
        Assert.Equal("Unexpected error message", errorResponse.Details);
    }

    [Fact]
    public async Task GetClubPlayers_ValidRequest_ReturnsOkWithPlayers()
    {
        // Arrange
        var expectedPlayers = new PaginatedResult<PlayerResponseDto>(
            new List<PlayerResponseDto>
            {
                new PlayerResponseDto
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Position = Position.Forward
                }
            },
            1,
            1,
            10
        );
        _clubService.Setup(x => x.GetClubPlayers(1, null, null, null, null, null))
            .ReturnsAsync(expectedPlayers);

        // Act
        var result = await _sut.GetClubPlayers(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<PaginatedResult<PlayerResponseDto>>(okResult.Value);
        Assert.Single(response.Items);
        Assert.Equal("John", response.Items.First().FirstName);
    }

    [Fact]
    public async Task GetClubPlayers_InvalidOperation_ReturnsBadRequest()
    {
        // Arrange
        _clubService.Setup(x => x.GetClubPlayers(1, null, null, null, null, null))
            .ThrowsAsync(new InvalidOperationException("Invalid operation message"));

        // Act
        var result = await _sut.GetClubPlayers(1);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
        Assert.Equal("Invalid operation message", errorResponse.Message);
    }

    [Fact]
    public async Task GetClubPlayers_UnexpectedError_ReturnsInternalServerError()
    {
        // Arrange
        _clubService.Setup(x => x.GetClubPlayers(1, null, null, null, null, null))
            .ThrowsAsync(new Exception("Unexpected error message"));

        // Act
        var result = await _sut.GetClubPlayers(1);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        var errorResponse = Assert.IsType<ErrorResponse>(statusCodeResult.Value);
        Assert.Equal("An error occurred while processing your request", errorResponse.Message);
        Assert.Equal("Unexpected error message", errorResponse.Details);
    }

    [Fact]
    public async Task GetClubPlayers_WithFilters_ReturnsFilteredResults()
    {
        // Arrange
        var name = "John";
        var position = Position.Forward;
        var minSalary = 30000m;
        var maxSalary = 50000m;
        var paginationParams = new PaginationParams { PageNumber = 1, PageSize = 10 };

        var players = new PaginatedResult<PlayerResponseDto>(
            new List<PlayerResponseDto>
            {
                new PlayerResponseDto
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Position = Position.Forward
                }
            },
            1,
            1,
            10
        );

        _clubService.Setup(x => x.GetClubPlayers(1, name, position, minSalary, maxSalary, paginationParams))
            .ReturnsAsync(players);

        // Act
        var result = await _sut.GetClubPlayers(1, name, position, minSalary, maxSalary, paginationParams);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<PaginatedResult<PlayerResponseDto>>(okResult.Value);
        Assert.Single(response.Items);
        _clubService.Verify(x => x.GetClubPlayers(1, name, position, minSalary, maxSalary, paginationParams), Times.Once);
    }

    [Fact]
    public async Task AddCoachToClub_InvalidOperation_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateCoachDto
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now),
            Salary = 75000m,
            Email = "john.doe@test.com"
        };
        _clubService.Setup(x => x.AddCoachToClub(1, request))
            .ThrowsAsync(new InvalidOperationException("Invalid operation message"));

        // Act
        var result = await _sut.AddCoachToClub(1, request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
        Assert.Equal("Invalid operation message", errorResponse.Message);
    }

    [Fact]
    public async Task AddCoachToClub_UnexpectedError_ReturnsInternalServerError()
    {
        // Arrange
        var request = new CreateCoachDto
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now),
            Salary = 75000m,
            Email = "john.doe@test.com"
        };
        _clubService.Setup(x => x.AddCoachToClub(1, request))
            .ThrowsAsync(new Exception("Unexpected error message"));

        // Act
        var result = await _sut.AddCoachToClub(1, request);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        var errorResponse = Assert.IsType<ErrorResponse>(statusCodeResult.Value);
        Assert.Equal("An error occurred while processing your request", errorResponse.Message);
        Assert.Equal("Unexpected error message", errorResponse.Details);
    }
}