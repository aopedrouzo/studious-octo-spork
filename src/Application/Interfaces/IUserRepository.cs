using FootballManager.Domain.Entities;

namespace FootballManager.Application.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<RefreshToken?> GetRefreshTokenAsync(string username, string refreshToken);
} 