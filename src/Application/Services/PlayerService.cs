using FootballManager.Application.DTOs;
using FootballManager.Application.DTOs.Request;
using FootballManager.Application.Interfaces;
using FootballManager.Application.Mappers;
using FootballManager.Domain.Entities;

namespace FootballManager.Application.Services;

public class PlayerService : IPlayerService
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IClubRepository _clubRepository;
    private readonly INotificationService _notificationService;

    public PlayerService(
        IPlayerRepository playerRepository,
        IClubRepository clubRepository,
        INotificationService notificationService)
    {
        _playerRepository = playerRepository;
        _clubRepository = clubRepository;
        _notificationService = notificationService;
    }


    public async Task<PlayerResponseDto> GetPlayerById(int id)
    {
        var player = await _playerRepository.GetByIdAsync(id);
        if (player == null)
            throw new KeyNotFoundException($"Player with ID {id} not found");

        return player.ToDto();
    }

    public async Task<PlayerResponseDto> ReleasePlayer(int playerId)
    {
        var player = await _playerRepository.GetByIdAsync(playerId);
        if (player == null)
            throw new KeyNotFoundException($"Player with ID {playerId} not found");

        if (player.ClubId != null)
        {
            var clubOrigin = await _clubRepository.GetByIdAsync(player.ClubId.Value);
            if (clubOrigin != null)
            {
                clubOrigin.HandleRelease(player);
                await _clubRepository.UpdateAsync(clubOrigin);
            }
        }

        player.LeaveClub();
        await _playerRepository.UpdateAsync(player);
        _ = SendReleaseNotification(player);
        return player.ToDto();
    }

    public async Task<PlayerResponseDto> TransferPlayer(int playerId, int clubId)
    {
        var player = await _playerRepository.GetByIdAsync(playerId);
        if (player == null)
            throw new KeyNotFoundException($"Player with ID {playerId} not found");

        if (player.ClubId != null)
        {
            var clubOrigin = await _clubRepository.GetByIdAsync(player.ClubId.Value);
            if (clubOrigin != null)
            {
                clubOrigin.HandleRelease(player);
                await _clubRepository.UpdateAsync(clubOrigin);
                player.LeaveClub();
            }
        }

        var club = await _clubRepository.GetByIdAsync(clubId);
        if (club == null)
            throw new KeyNotFoundException($"Club with ID {clubId} not found");

        club.HandleTransfer(player);
        await _clubRepository.UpdateAsync(club);

        player.JoinClub(club);
        await _playerRepository.UpdateAsync(player);
        _ = SendTransferNotification(player, club);
        return player.ToDto();
    }

    public async Task<PlayerResponseDto> AddPlayer(CreatePlayerDto request)
    {
        var player = request.ToEntity();

        await _playerRepository.AddAsync(player);
        return player.ToDto();
    }

    private async Task SendTransferNotification(Player player, Club club)
    {
        var notification = new NotificationMessage
        {
            RecipientId = player.Id.ToString(),
            Channel = NotificationChannel.Email,
            Title = "Transfer Notification",
            Body = $"Dear {player.FirstName} {player.LastName}, your transfer to {club.Name} has been processed.",
            CreatedAt = DateTime.UtcNow,
            AdditionalData = new Dictionary<string, string>
            {
                { "PlayerName", $"{player.FirstName} {player.LastName}" },
                { "ClubName", club.Name }
            }
        };

        await _notificationService.SendNotificationAsync(notification);
    }

    private async Task SendReleaseNotification(Player player)
    {
        var notification = new NotificationMessage
        {
            RecipientId = player.Id.ToString(),
            Channel = NotificationChannel.Email,
            Title = "Release Notification",
            Body = $"Dear {player.FirstName} {player.LastName}, you have been released from your current club.",
            CreatedAt = DateTime.UtcNow,
            AdditionalData = new Dictionary<string, string>
            {
                { "PlayerName", $"{player.FirstName} {player.LastName}" }
            }
        };

        await _notificationService.SendNotificationAsync(notification);
    }
}