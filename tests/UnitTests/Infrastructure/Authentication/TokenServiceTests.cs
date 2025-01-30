using FootballManager.Infrastructure.Authentication;
using FootballManager.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace FootballClubManagerTests.UnitTests.Infrastructure.Authentication;

public class TokenServiceTests
{
    private readonly Mock<IConfiguration> _configuration;
    private readonly Mock<IUserRepository> _userRepository;
    private readonly TokenService _sut;
    private readonly string _testSecret = "your-256-bit-secret-your-256-bit-secret-your-256-bit-secret";

    public TokenServiceTests()
    {
        _configuration = new Mock<IConfiguration>();
        _userRepository = new Mock<IUserRepository>();
        
        _configuration.Setup(x => x["Jwt:Secret"]).Returns(_testSecret);
        
        _sut = new TokenService(_configuration.Object, _userRepository.Object);
    }

    [Fact]
    public void GenerateTokens_ValidInputs_ReturnsTokenPair()
    {
        // Arrange
        var userId = "testuser";
        var role = "User";

        // Act
        var (accessToken, refreshToken) = _sut.GenerateTokens(userId, role);

        // Assert
        Assert.NotNull(accessToken);
        Assert.NotNull(refreshToken);
        Assert.NotEmpty(accessToken);
        Assert.NotEmpty(refreshToken);
        
        // Verify the JWT token structure
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(accessToken);
        
        Assert.Equal(userId, jwtToken.Claims.First(c => c.Type == "nameid").Value);
        Assert.Equal(role, jwtToken.Claims.First(c => c.Type == "role").Value);
    }

    [Fact]
    public void ValidateToken_ValidToken_ReturnsClaimsPrincipal()
    {
        // Arrange
        var userId = "testuser";
        var role = "User";
        var (accessToken, _) = _sut.GenerateTokens(userId, role);

        // Act
        var principal = _sut.ValidateToken(accessToken);

        // Assert
        Assert.NotNull(principal);
        Assert.Equal(userId, principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        Assert.Equal(role, principal.FindFirst(ClaimTypes.Role)?.Value);
    }



    [Fact]
    public void GenerateTokens_SameInput_GeneratesDifferentRefreshTokens()
    {
        // Arrange
        var userId = "testuser";
        var role = "User";

        // Act
        var (_, refreshToken1) = _sut.GenerateTokens(userId, role);
        var (_, refreshToken2) = _sut.GenerateTokens(userId, role);

        // Assert
        Assert.NotEqual(refreshToken1, refreshToken2);
    }

    [Fact]
    public void GenerateTokens_ValidInput_GeneratesCorrectTokenExpiration()
    {
        // Arrange
        var userId = "testuser";
        var role = "User";

        // Act
        var (accessToken, _) = _sut.GenerateTokens(userId, role);
        
        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(accessToken);
        
        var expectedExpiration = DateTime.UtcNow.AddMinutes(15);
        var actualExpiration = jwtToken.ValidTo;
        
        // Allow for 1 minute difference due to test execution time
        Assert.True(Math.Abs((expectedExpiration - actualExpiration).TotalMinutes) < 1);
    }
} 