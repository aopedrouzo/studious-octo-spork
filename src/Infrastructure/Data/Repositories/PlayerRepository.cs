using FootballManager.Application.Common;
using FootballManager.Application.Interfaces;
using FootballManager.Domain.Entities;
using FootballManager.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FootballManager.Infrastructure.Data.Repositories;

public class PlayerRepository : PostgreRepository<Player>, IPlayerRepository
{
    public PlayerRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<PaginatedResult<Player>> GetFilteredPlayersAsync(
        int? clubId = null,
        string? name = null,
        Position? position = null,
        decimal? minSalary = null,
        decimal? maxSalary = null,
        PaginationParams? paginationParams = null)
    {
        var query = _context.Set<Player>().AsQueryable();

        if (clubId.HasValue)
            query = query.Where(p => p.ClubId == clubId);

        if (!string.IsNullOrEmpty(name))
            query = query.Where(p => 
                EF.Functions.ILike(p.FirstName + " " + p.LastName, $"%{name}%"));

        if (position.HasValue)
            query = query.Where(p => p.Position == position);

        if (minSalary.HasValue)
            query = query.Where(p => p.Salary >= minSalary);

        if (maxSalary.HasValue)
            query = query.Where(p => p.Salary <= maxSalary);

        var totalCount = await query.CountAsync();

        var page = paginationParams?.PageNumber ?? 1;
        var pageSize = paginationParams?.PageSize ?? 10;
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<Player>(items, totalCount, page, pageSize);
    }
}