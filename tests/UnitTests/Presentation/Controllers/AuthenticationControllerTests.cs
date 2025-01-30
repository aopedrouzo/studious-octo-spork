using FootballManager.Application.DTOs;
using FootballManager.Application.Interfaces;
using FootballManager.Presentation.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace FootballClubManagerTests.UnitTests.Presentation.Controllers;

public class AuthenticationControllerTests
{
    private readonly Mock<IUserService> _userService;
    private readonly AuthenticationController _sut;

    public AuthenticationControllerTests()
    {
        _userService = new Mock<IUserService>();
        _sut = new AuthenticationController(_userService.Object);
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

    private static AuthenticationResponseDto CreateTestAuthResponse(
        string token = "test_token",
        string refreshToken = "refresh_token",
        int expiresIn = 900)
    {
        return new AuthenticationResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            ExpiresIn = expiresIn
        };
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkWithToken()
    {
        // Arrange
        var request = CreateTestLoginRequest();
        var expectedResponse = CreateTestAuthResponse();
        _userService.Setup(x => x.AuthenticateAsync(request))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _sut.Login(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<AuthenticationResponseDto>(okResult.Value);
        Assert.Equal(expectedResponse.Token, response.Token);
        Assert.Equal(expectedResponse.RefreshToken, response.RefreshToken);
        Assert.Equal(expectedResponse.ExpiresIn, response.ExpiresIn);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsBadRequest()
    {
        // Arrange
        var request = CreateTestLoginRequest();
        _userService.Setup(x => x.AuthenticateAsync(request))
            .ThrowsAsync(new InvalidOperationException("Invalid credentials"));

        // Act
        var result = await _sut.Login(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
        Assert.Equal("Invalid credentials", errorResponse.Message);
    }

    [Fact]
    public async Task Login_UnexpectedError_ReturnsInternalServerError()
    {
        // Arrange
        var request = CreateTestLoginRequest();
        _userService.Setup(x => x.AuthenticateAsync(request))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _sut.Login(request);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        var errorResponse = Assert.IsType<ErrorResponse>(statusCodeResult.Value);
        Assert.Equal("An error occurred while processing your request", errorResponse.Message);
    }

    [Fact]
    public async Task RefreshToken_ValidToken_ReturnsOkWithNewToken()
    {
        // Arrange
        var request = new RefreshTokenRequest
        {
            ExpiredToken = "expired_token",
            RefreshToken = "refresh_token"
        };
        var expectedResponse = CreateTestAuthResponse("new_token", "new_refresh_token");
        _userService.Setup(x => x.RefreshTokenAsync(request))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _sut.RefreshToken(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<AuthenticationResponseDto>(okResult.Value);
        Assert.Equal(expectedResponse.Token, response.Token);
        Assert.Equal(expectedResponse.RefreshToken, response.RefreshToken);
        Assert.Equal(expectedResponse.ExpiresIn, response.ExpiresIn);
    }

    [Fact]
    public async Task RefreshToken_InvalidToken_ReturnsBadRequest()
    {
        // Arrange
        var request = new RefreshTokenRequest
        {
            ExpiredToken = "invalid_token",
            RefreshToken = "invalid_refresh_token"
        };
        _userService.Setup(x => x.RefreshTokenAsync(request))
            .ThrowsAsync(new SecurityTokenException("Invalid token"));

        // Act
        var result = await _sut.RefreshToken(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
        Assert.Equal("Invalid or expired refresh token", errorResponse.Message);
    }

    [Fact]
    public async Task RefreshToken_UnexpectedError_ReturnsInternalServerError()
    {
        // Arrange
        var request = new RefreshTokenRequest
        {
            ExpiredToken = "expired_token",
            RefreshToken = "refresh_token"
        };
        _userService.Setup(x => x.RefreshTokenAsync(request))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _sut.RefreshToken(request);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        var errorResponse = Assert.IsType<ErrorResponse>(statusCodeResult.Value);
        Assert.Equal("An error occurred while processing your request", errorResponse.Message);
    }
}
