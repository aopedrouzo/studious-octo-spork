using FootballManager.Application.Interfaces;
using FootballManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FootballManager.Infrastructure.Data.Repositories;

public class UserRepository : PostgreRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string username, string refreshToken)
    {
        return await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == refreshToken && t.Username == username);
    }
    
} 
