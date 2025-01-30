namespace FootballManager.Application.Interfaces;
using System.Security.Claims;

public interface ITokenService
{
    (string AccessToken, string RefreshToken) GenerateTokens(string userId, string role);
    ClaimsPrincipal? ValidateToken(string token);
}
