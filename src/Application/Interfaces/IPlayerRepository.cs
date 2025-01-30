using FootballManager.Application.Common;
using FootballManager.Domain.Entities;
using FootballManager.Domain.Enums;

namespace FootballManager.Application.Interfaces;

public interface IPlayerRepository : IRepository<Player>
{
    Task<PaginatedResult<Player>> GetFilteredPlayersAsync(
        int? clubId = null,
        string? name = null,
        Position? position = null,
        decimal? minSalary = null,
        decimal? maxSalary = null,
        PaginationParams? paginationParams = null);
}