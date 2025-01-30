using FootballManager.Application.Interfaces;
using FootballManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FootballManager.Infrastructure.Data.Repositories;

public class ClubRepository : PostgreRepository<Club>, IClubRepository
{
    public ClubRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<decimal> GetTotalSalaryAsync(int clubId)
    {
        var club = await _context.Clubs
            .Include(c => c.Players)
            .Include(c => c.Coaches)
            .FirstOrDefaultAsync(c => c.Id == clubId);

        if (club == null)
            throw new KeyNotFoundException($"Club with ID {clubId} not found");

        var playersSalary = club.Players?.Sum(p => p.Salary) ?? 0;
        var coachesSalary = club.Coaches?.Sum(c => c.Salary) ?? 0;

        return playersSalary + coachesSalary;
    }
}