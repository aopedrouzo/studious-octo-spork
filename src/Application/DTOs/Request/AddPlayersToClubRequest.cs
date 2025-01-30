using FootballManager.Application.DTOs.Request;
using FootballManager.Domain.Enums;

namespace FootballManager.Application.DTOs;

public class AddPlayersToClubRequest
{
    public CreatePlayerDto[] Players { get; set; } = Array.Empty<CreatePlayerDto>();
}

