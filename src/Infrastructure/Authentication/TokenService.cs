using FootballManager.Application.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;

namespace FootballManager.Infrastructure.Authentication;

public class TokenService : ITokenService
{
    private readonly string _jwtSecret;
    private readonly IConfiguration _configuration;

    public TokenService(
        IConfiguration configuration,
        IUserRepository userRepository)
    {
        _configuration = configuration;
        _jwtSecret = configuration["Jwt:Secret"]!;
    }

    public (string AccessToken, string RefreshToken) GenerateTokens(string userId, string role)
    {
        var accessToken = GenerateAccessToken(userId, role);
        var refreshToken = GenerateRefreshToken();
        
        return (accessToken, refreshToken);
    }

    private string GenerateAccessToken(string userId, string role)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSecret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, role)
            }),
            Expires = DateTime.UtcNow.AddMinutes(15), // Shorter lifetime for access tokens
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSecret);

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return principal;
        }
        catch
        {
            return null;
        }
    }
}