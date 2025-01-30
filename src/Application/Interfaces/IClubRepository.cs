using FootballManager.Domain.Entities;

namespace FootballManager.Application.Interfaces;

public interface IClubRepository : IRepository<Club>
{
    Task<decimal> GetTotalSalaryAsync(int clubId);
}
