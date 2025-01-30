using FootballManager.Application.DTOs;
using FootballManager.Application.Interfaces;
using FootballManager.Application.Services;
using FootballManager.Domain.Entities;
using Moq;
using System.Security.Claims;
using BCrypt.Net;

namespace FootballClubManagerTests.UnitTests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepository;
    private readonly Mock<ITokenService> _tokenService;
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _userRepository = new Mock<IUserRepository>();
        _tokenService = new Mock<ITokenService>();
        _sut = new UserService(_userRepository.Object, _tokenService.Object);
    }

    private static User CreateTestUser(
        string username = "testuser",
        string role = "User",
        string passwordHash = "$2a$11$QR8Vz.71TFQF1UgK.3o/OeXD.7/1AO1oA.0FyQeR4ExLYe4eJpKoq") // hash for "password123"
    {
        return new User
        {
            Username = username,
            Role = role,
            PasswordHash = passwordHash,
            RefreshTokens = new List<RefreshToken>()
        };
    }

    private static LoginRequestDto CreateTestLoginRequest(
        string username = "testuser",
        string password = "password123")
    {
        return new LoginRequestDto
        {
            Username = username,
            Password = password
        };
    }

    [Fact]
    public async Task AuthenticateAsync_ValidCredentials_ReturnsAuthenticationResponse()
    {
        // Arrange
        var password = "password123";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        var user = CreateTestUser(passwordHash: passwordHash);
        var request = CreateTestLoginRequest(password: password);
        var accessToken = "access_token";
        var refreshToken = "refresh_token";

        _userRepository.Setup(x => x.GetByUsernameAsync(request.Username)).ReturnsAsync(user);
        _tokenService.Setup(x => x.GenerateTokens(user.Username, user.Role))
            .Returns((accessToken, refreshToken));

        // Act
        var result = await _sut.AuthenticateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(accessToken, result.Token);
        Assert.Equal(refreshToken, result.RefreshToken);
        Assert.Equal(900, result.ExpiresIn);
        _userRepository.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Exactly(2));
    }

    [Fact]
    public async Task AuthenticateAsync_InvalidCredentials_ThrowsInvalidOperationException()
    {
        // Arrange
        var request = CreateTestLoginRequest();
        _userRepository.Setup(x => x.GetByUsernameAsync(request.Username))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.AuthenticateAsync(request));
    }

    [Fact]
    public async Task RefreshTokenAsync_ValidTokens_ReturnsNewAuthenticationResponse()
    {
        // Arrange
        var user = CreateTestUser();
        var expiredToken = "expired_token";
        var refreshToken = "refresh_token";
        var newAccessToken = "new_access_token";
        var newRefreshToken = "new_refresh_token";
        var claims = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Username) });
        var claimsPrincipal = new ClaimsPrincipal(claims);

        _tokenService.Setup(x => x.ValidateToken(expiredToken)).Returns(claimsPrincipal);
        _userRepository.Setup(x => x.GetByUsernameAsync(user.Username)).ReturnsAsync(user);
        _userRepository.Setup(x => x.GetRefreshTokenAsync(user.Username, refreshToken))
            .ReturnsAsync(new RefreshToken 
            {
                Token = refreshToken,
                Username = user.Username,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30)
            });
        _tokenService.Setup(x => x.GenerateTokens(user.Username, user.Role))
            .Returns((newAccessToken, newRefreshToken));

        var request = new RefreshTokenRequest
        {
            ExpiredToken = expiredToken,
            RefreshToken = refreshToken
        };

        // Act
        var result = await _sut.RefreshTokenAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newAccessToken, result.Token);
        Assert.Equal(newRefreshToken, result.RefreshToken);
        Assert.Equal(900, result.ExpiresIn);
        
        // Verify that UpdateAsync was called once to store the new refresh token
        _userRepository.Verify(x => x.UpdateAsync(It.Is<User>(u => 
            u.RefreshTokens.Any(t => 
                t.Token == newRefreshToken && 
                t.Username == user.Username && 
                t.ExpiresAt > DateTime.UtcNow))), 
            Times.Once);
    }

    [Fact]
    public async Task RefreshTokenAsync_InvalidExpiredToken_ThrowsInvalidOperationException()
    {
        // Arrange
        _tokenService.Setup(x => x.ValidateToken(It.IsAny<string>()))
            .Returns((ClaimsPrincipal?)null);

        var request = new RefreshTokenRequest
        {
            ExpiredToken = "invalid_token",
            RefreshToken = "refresh_token"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.RefreshTokenAsync(request));
    }

    [Fact]
    public async Task RefreshTokenAsync_ExpiredRefreshToken_ThrowsInvalidOperationException()
    {
        // Arrange
        var user = CreateTestUser();
        var claims = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Username) });
        var claimsPrincipal = new ClaimsPrincipal(claims);

        _tokenService.Setup(x => x.ValidateToken(It.IsAny<string>())).Returns(claimsPrincipal);
        _userRepository.Setup(x => x.GetByUsernameAsync(user.Username)).ReturnsAsync(user);
        _userRepository.Setup(x => x.GetRefreshTokenAsync(user.Username, It.IsAny<string>()))
            .ReturnsAsync(new RefreshToken 
            { 
                Token = "expired_token",
                Username = user.Username,
                ExpiresAt = DateTime.UtcNow.AddMinutes(-30) // expired
            });

        var request = new RefreshTokenRequest
        {
            ExpiredToken = "expired_token",
            RefreshToken = "refresh_token"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.RefreshTokenAsync(request));
    }
} 