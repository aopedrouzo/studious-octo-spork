using FootballManager.Application.DTOs;

namespace FootballManager.Application.Interfaces;

public interface IUserService
{
    Task<AuthenticationResponseDto> AuthenticateAsync(LoginRequestDto request);
    Task<AuthenticationResponseDto> RefreshTokenAsync(RefreshTokenRequest request);
} 