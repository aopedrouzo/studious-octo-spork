using FootballManager.Application.DTOs;
using FootballManager.Application.DTOs.Request;
using FootballManager.Domain.Entities;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace FootballManager.Application.Interfaces;

public interface IPlayerService
{
    Task<PlayerResponseDto> GetPlayerById(int id);
    Task<PlayerResponseDto> ReleasePlayer(int playerId);
    Task<PlayerResponseDto> TransferPlayer(int playerId, int clubId);
    Task<PlayerResponseDto> AddPlayer(CreatePlayerDto request);
    
}
