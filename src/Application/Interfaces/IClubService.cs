using FootballManager.Application.Common;
using FootballManager.Application.DTOs;
using FootballManager.Application.DTOs.Request;
using FootballManager.Application.DTOs.Response;
using FootballManager.Domain.Entities;
using FootballManager.Domain.Enums;

namespace FootballManager.Application.Interfaces;

public interface IClubService
{
    Task<ClubListResponseDto> GetAllClubs();
    Task<ClubResponseDto> GetClubById(int id);
    Task<ClubResponseDto> CreateClub(CreateClubDto request);
    Task<ClubResponseDto> AddPlayersToClub(int clubId, AddPlayersToClubRequest request);
    Task<AddCoachToClubResponseDto> AddCoachToClub(int clubId, CreateCoachDto request);
    Task<ClubResponseDto> AdjustClubBudget(int clubId, UpdateClubBudgetRequest request);
    Task<PaginatedResult<PlayerResponseDto>> GetClubPlayers(
        int clubId,
        string? name = null,
        Position? position = null,
        decimal? minSalary = null,
        decimal? maxSalary = null,
        PaginationParams? paginationParams = null);
        
        
}