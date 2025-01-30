using FootballManager.Application.Common;
using FootballManager.Application.DTOs;
using FootballManager.Application.DTOs.Request;
using FootballManager.Application.Interfaces;
using FootballManager.Application.Services;
using FootballManager.Domain.Entities;
using FootballManager.Domain.Enums;
using Moq;

namespace FootballClubManagerTests.UnitTests.Services;

public class ClubServiceTests
{
    private readonly Mock<IClubRepository> _clubRepository;
    private readonly Mock<IPlayerRepository> _playerRepository;
    private readonly Mock<ICoachRepository> _coachRepository;
    private readonly Mock<INotificationService> _notificationService;
    private readonly ClubService _sut;

    public ClubServiceTests()
    {
        _clubRepository = new Mock<IClubRepository>();
        _playerRepository = new Mock<IPlayerRepository>();
        _coachRepository = new Mock<ICoachRepository>();
        _notificationService = new Mock<INotificationService>();
        _sut = new ClubService(_clubRepository.Object, _playerRepository.Object, _coachRepository.Object, _notificationService.Object);
    }

    private static Club CreateTestClub(
        string name = "Test FC",
        decimal budget = 1000000m)
    {
        return new Club(name, budget);
    }

    private static CreateClubDto CreateTestClubDto(
        string name = "Test FC",
        decimal budget = 1000000m)
    {
        return new CreateClubDto
        {
            Name = name,
            Budget = budget
        };
    }

    private static CreateCoachDto CreateTestCoachDto(
        string firstName = "John",
        string lastName = "Doe",
        decimal salary = 50000m,
        string email = "john.doe@test.com")
    {
        return new CreateCoachDto
        {
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now),
            Salary = salary,
            Email = email
        };
    }

    private static CreatePlayerDto CreateTestPlayerDto(
        string firstName = "John",
        string lastName = "Smith",
        decimal salary = 30000m,
        string email = "john.smith@test.com",
        Position position = Position.Forward)
    {
        return new CreatePlayerDto
        {
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now),
            Salary = salary,
            Email = email,
            Position = position
        };
    }

    [Fact]
    public async Task GetAllClubs_ReturnsClubListResponse()
    {
        // Arrange
        var clubs = new List<Club> { CreateTestClub(), CreateTestClub("Another FC") };
        _clubRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(clubs);

        // Act
        var result = await _sut.GetAllClubs();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Clubs.Count());
    }

    [Fact]
    public async Task GetClubById_ExistingClub_ReturnsClubDto()
    {
        // Arrange
        var club = CreateTestClub();
        _clubRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(club);

        // Act
        var result = await _sut.GetClubById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(club.Name, result.Name);
        Assert.Equal(club.Budget, result.Budget);
    }

    [Fact]
    public async Task CreateClub_ValidRequest_ReturnsClubDto()
    {
        // Arrange
        var request = CreateTestClubDto();
        var club = CreateTestClub();
        _clubRepository.Setup(x => x.AddAsync(It.IsAny<Club>())).ReturnsAsync(club);

        // Act
        var result = await _sut.CreateClub(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Name, result.Name);
        Assert.Equal(request.Budget, result.Budget);
        _clubRepository.Verify(x => x.AddAsync(It.IsAny<Club>()), Times.Once);
    }

    [Fact]
    public async Task AddCoachToClub_ValidRequest_ReturnsAddCoachToClubResponse()
    {
        // Arrange
        var club = CreateTestClub();
        var request = CreateTestCoachDto();
        _clubRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(club);
        _clubRepository.Setup(x => x.UpdateAsync(It.IsAny<Club>())).Returns(Task.CompletedTask);
        _coachRepository.Setup(x => x.AddAsync(It.IsAny<Coach>())).ReturnsAsync(new Coach(
            request.FirstName, request.LastName, request.DateOfBirth, request.Salary, request.Email));

        // Act
        var result = await _sut.AddCoachToClub(1, request);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Club);
        Assert.NotNull(result.Coach);
        Assert.Equal(request.FirstName, result.Coach.FirstName);
        Assert.Equal(request.LastName, result.Coach.LastName);
        _notificationService.Verify(x => x.SendNotificationAsync(It.IsAny<NotificationMessage>()), Times.Once);
    }

    [Fact]
    public async Task AdjustClubBudget_ValidRequest_ReturnsUpdatedClubDto()
    {
        // Arrange
        var club = CreateTestClub();
        var request = new UpdateClubBudgetRequest { Amount = 500000m };
        _clubRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(club);
        _clubRepository.Setup(x => x.GetTotalSalaryAsync(1)).ReturnsAsync(100000m);
        _clubRepository.Setup(x => x.UpdateAsync(It.IsAny<Club>())).Returns(Task.CompletedTask);

        // Act
        var result = await _sut.AdjustClubBudget(1, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(club.Name, result.Name);
        _clubRepository.Verify(x => x.UpdateAsync(It.IsAny<Club>()), Times.Once);
    }

    [Fact]
    public async Task GetClubById_NonExistingClub_ThrowsKeyNotFoundException()
    {
        // Arrange
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        _clubRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Club?)null);
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetClubById(1));
    }

    [Fact]
    public async Task AddCoachToClub_NonExistingClub_ThrowsKeyNotFoundException()
    {
        // Arrange
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        _clubRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Club?)null);
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        var request = CreateTestCoachDto();

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.AddCoachToClub(1, request));
    }

    [Fact]
    public async Task AdjustClubBudget_NonExistingClub_ThrowsKeyNotFoundException()
    {
        // Arrange
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        _clubRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Club?)null);
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        var request = new UpdateClubBudgetRequest { Amount = 500000m };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.AdjustClubBudget(1, request));
    }

    [Fact]
    public async Task AddPlayersToClub_ValidRequest_ReturnsClubDto()
    {
        // Arrange
        var club = CreateTestClub();
        var players = new[]
        {
            CreateTestPlayerDto(),
            CreateTestPlayerDto("Jane", "Doe", 40000m, "jane.doe@test.com", Position.Midfielder)
        };
        var request = new AddPlayersToClubRequest { Players = players };

        _clubRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(club);
        _clubRepository.Setup(x => x.UpdateAsync(It.IsAny<Club>())).Returns(Task.CompletedTask);
        _playerRepository.Setup(x => x.AddRangeAsync(It.IsAny<IEnumerable<Player>>())).Returns(Task.CompletedTask);

        // Act
        var result = await _sut.AddPlayersToClub(1, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(club.Name, result.Name);
        _clubRepository.Verify(x => x.UpdateAsync(It.IsAny<Club>()), Times.Once);
        _playerRepository.Verify(x => x.AddRangeAsync(It.IsAny<IEnumerable<Player>>()), Times.Once);
        _notificationService.Verify(x => x.SendNotificationAsync(It.IsAny<NotificationMessage>()), Times.Exactly(2));
    }

    [Fact]
    public async Task AddPlayersToClub_NonExistingClub_ThrowsKeyNotFoundException()
    {
        // Arrange
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        _clubRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Club?)null);
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        var request = new AddPlayersToClubRequest 
        { 
            Players = new[] { CreateTestPlayerDto() } 
        };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.AddPlayersToClub(1, request));
    }

    [Fact]
    public async Task GetClubPlayers_ValidRequest_ReturnsPaginatedPlayers()
    {
        // Arrange
        var club = CreateTestClub();
        var players = new List<Player>
        {
            new Player("John", "Doe", DateOnly.FromDateTime(DateTime.Now), Position.Forward, 50000m, "john.doe@test.com"),
            new Player("Jane", "Smith", DateOnly.FromDateTime(DateTime.Now), Position.Midfielder, 45000m, "jane.smith@test.com")
        };
        
        var expectedPlayers = new PaginatedResult<Player>(
            players,           // Use actual player list
            2,                // Total count
            1,                // Page number
            10                // Page size
        );

        _clubRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(club);
        _playerRepository.Setup(x => x.GetFilteredPlayersAsync(
            1, null, null, null, null, null))
            .ReturnsAsync(expectedPlayers);

        // Act
        var result = await _sut.GetClubPlayers(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count());
        Assert.Equal("John", result.Items.First().FirstName);
        Assert.Equal("Jane", result.Items.Last().FirstName);
    }

    [Fact]
    public async Task GetClubPlayers_NonExistingClub_ThrowsKeyNotFoundException()
    {
        // Arrange
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        _clubRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Club?)null);
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetClubPlayers(1));
    }
} 