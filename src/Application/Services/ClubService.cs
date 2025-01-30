using FootballManager.Application.Common;
using FootballManager.Application.DTOs;
using FootballManager.Application.DTOs.Request;
using FootballManager.Application.DTOs.Response;
using FootballManager.Application.Interfaces;
using FootballManager.Application.Mappers;
using FootballManager.Domain.Entities;
using FootballManager.Domain.Enums;

namespace FootballManager.Application.Services;

public class ClubService : IClubService
{
    private readonly IClubRepository _clubRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly ICoachRepository _coachRepository;
    private readonly INotificationService _notificationService;

    public ClubService(IClubRepository clubRepository, IPlayerRepository playerRepository, ICoachRepository coachRepository, INotificationService notificationService)
    {
        _clubRepository = clubRepository;
        _playerRepository = playerRepository;
        _coachRepository = coachRepository;
        _notificationService = notificationService;
    }

    public async Task<ClubListResponseDto> GetAllClubs()
    {
        var clubs = await _clubRepository.GetAllAsync();
        return clubs.ToListResponse();
    }

    public async Task<ClubResponseDto> GetClubById(int id)
    {
        var club = await _clubRepository.GetByIdAsync(id);
        if (club == null)
        {
            throw new KeyNotFoundException($"Club with ID {id} not found");
        }
        return club.ToDto();
    }

    public async Task<ClubResponseDto> CreateClub(CreateClubDto request)
    {
        var club = request.ToEntity();
        var createdClub = await _clubRepository.AddAsync(club);
        return createdClub.ToDto();
    }

    public async Task<ClubResponseDto> AddPlayersToClub(int clubId, AddPlayersToClubRequest request)
    {
        var club = await _clubRepository.GetByIdAsync(clubId);
        if (club == null)
        {
            throw new KeyNotFoundException($"Club with ID {clubId} not found");
        }

        var players = request.ToEntities().ToList();
        foreach (var player in players)
        {
            club.HandleTransfer(player);
            player.JoinClub(club);
            _ = SendTransferNotification(player, club); 
        }

        await _clubRepository.UpdateAsync(club);
        await _playerRepository.AddRangeAsync(players);
        return club.ToDto();
    }

    public async Task<AddCoachToClubResponseDto> AddCoachToClub(int clubId, CreateCoachDto request)
    {
        var club = await _clubRepository.GetByIdAsync(clubId);
        if (club == null)
        {
            throw new KeyNotFoundException($"Club with ID {clubId} not found");
        }

        var coach = request.ToEntity();
        club.HandleTransfer(coach);
        coach.JoinClub(club);

        await _clubRepository.UpdateAsync(club);
        await _coachRepository.AddAsync(coach);
        _ = SendTransferNotification(coach, club); // Fire and forget
        return club.ToAddCoachResponse(coach);
    }

    public async Task<ClubResponseDto> AdjustClubBudget(int clubId, UpdateClubBudgetRequest request)
    {
        var club = await _clubRepository.GetByIdAsync(clubId);
        if (club == null)
        {
            throw new KeyNotFoundException($"Club with ID {clubId} not found");
        }

        var totalSalary = await _clubRepository.GetTotalSalaryAsync(clubId);
        club.ModifyBudget(request.Amount, totalSalary);
        await _clubRepository.UpdateAsync(club);
        return club.ToDto();
    }

    public async Task<PaginatedResult<PlayerResponseDto>> GetClubPlayers(
        int clubId,
        string? name = null,
        Position? position = null,
        decimal? minSalary = null,
        decimal? maxSalary = null,
        PaginationParams? paginationParams = null)
    {
        var club = await _clubRepository.GetByIdAsync(clubId);
        if (club == null)
        {
            throw new KeyNotFoundException($"Club with ID {clubId} not found");
        }

        var result = await _playerRepository.GetFilteredPlayersAsync(
            clubId, name, position, minSalary, maxSalary, paginationParams);
        return result.ToDto();
    }

    private async Task SendTransferNotification(Employee employee, Club club)
    {
        var notification = new NotificationMessage
        {
            RecipientId = employee.Email,
            Channel = NotificationChannel.Email,
            Title = "Transfer Notification",
            Body = $"Dear {employee.FirstName + " " + employee.LastName}, your transfer to {club.Name} has been processed.",
            CreatedAt = DateTime.UtcNow,
            AdditionalData = new Dictionary<string, string>
            {
                { "EmployeeName", employee.FirstName + " " + employee.LastName },
                { "ClubName", club.Name }
            }
        };

        await _notificationService.SendNotificationAsync(notification);
    }
}