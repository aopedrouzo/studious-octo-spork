using FootballManager.Application.DTOs;
using FootballManager.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace FootballManager.Presentation.Controllers;

[Route("auth")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthenticationController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token
    /// </summary>
    /// <param name="request">The login credentials</param>
    /// <returns>JWT token for authenticated user</returns>
    /// <response code="200">Returns the JWT token</response>
    /// <response code="400">If the credentials are invalid</response>
    /// <response code="500">If there's an unexpected error</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthenticationResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var response = await _userService.AuthenticateAsync(request);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Invalid login attempt: {ex}");
            return BadRequest(new ErrorResponse("Invalid credentials"));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during login: {ex}");
            return StatusCode(500, new ErrorResponse(
                "An error occurred while processing your request",
                ex.Message));
        }
    }

    /// <summary>
    /// Refreshes an expired JWT token using a valid refresh token
    /// </summary>
    /// <param name="request">The refresh token request</param>
    /// <returns>New JWT token and refresh token</returns>
    /// <response code="200">Returns the new JWT token and refresh token</response>
    /// <response code="400">If the refresh token is invalid or expired</response>
    /// <response code="500">If there's an unexpected error</response>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthenticationResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var response = await _userService.RefreshTokenAsync(request);
            return Ok(response);
        }
        catch (SecurityTokenException ex)
        {
            Console.WriteLine($"Invalid refresh token attempt: {ex}");
            return BadRequest(new ErrorResponse("Invalid or expired refresh token"));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during token refresh: {ex}");
            return StatusCode(500, new ErrorResponse(
                "An error occurred while processing your request",
                ex.Message));
        }
    }
}
