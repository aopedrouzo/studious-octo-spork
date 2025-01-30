using FootballManager.Application.Interfaces;
using FootballManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FootballManager.Infrastructure.Data.Repositories;

public class CoachRepository : PostgreRepository<Coach>, ICoachRepository
{
    public CoachRepository(AppDbContext context) : base(context)
    {
    }   
}