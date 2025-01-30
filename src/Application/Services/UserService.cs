using System.Security.Claims;
using FootballManager.Application.DTOs;
using FootballManager.Application.Interfaces;
using FootballManager.Domain.Entities;


namespace FootballManager.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public UserService(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<AuthenticationResponseDto> AuthenticateAsync(LoginRequestDto request)
    {
        var user = await ValidateUserAsync(request.Username, request.Password);
        
        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        var (accessToken, refreshToken) = _tokenService.GenerateTokens(user.Username, user.Role);

        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            Username = user.Username,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };  
        user.RefreshTokens.Add(refreshTokenEntity); 
        await _userRepository.UpdateAsync(user);

        return new AuthenticationResponseDto
        {
            Token = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = 900 // 15 minutes in seconds
        };
    }

    public async Task<AuthenticationResponseDto> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var expiredClaim = _tokenService.ValidateToken(request.ExpiredToken)
            ?? throw new InvalidOperationException("Invalid expired token");

        var username = expiredClaim.FindFirstValue(ClaimTypes.NameIdentifier) 
            ?? throw new InvalidOperationException("Username claim not found");
        
        var user = await _userRepository.GetByUsernameAsync(username) 
            ?? throw new InvalidOperationException("User not found");

        var refreshToken = await ValidateRefreshTokenAsync(user, request.RefreshToken);

        var (accessToken, newRefreshToken) = _tokenService.GenerateTokens(user.Username, user.Role);

        var newRefreshTokenEntity = new RefreshToken
        {
            Token = newRefreshToken,
            Username = user.Username,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };
        user.RefreshTokens.Add(newRefreshTokenEntity);
        await _userRepository.UpdateAsync(user);

        return new AuthenticationResponseDto
        {
            Token = accessToken,
            RefreshToken = newRefreshToken,
            ExpiresIn = 900 // 15 minutes in seconds
        };
    }

    private async Task<User> ValidateUserAsync(string username, string password)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            throw new InvalidOperationException("Invalid username or password");
        }
        return user;
    }

    private async Task<RefreshToken> ValidateRefreshTokenAsync(User user, string refreshToken)
    {
        var token = await _userRepository.GetRefreshTokenAsync(user.Username, refreshToken);
        if (token == null || token.ExpiresAt < DateTime.UtcNow)
        {
            throw new InvalidOperationException("Invalid or expired refresh token");
        }
        return token;
    }
} 