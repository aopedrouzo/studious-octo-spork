using FootballManager.Application.Common;
using FootballManager.Application.DTOs;
using FootballManager.Application.DTOs.Request;
using FootballManager.Application.Interfaces;
using FootballManager.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballManager.Presentation.Controllers;

[Route("player")]
[ApiController]
public class PlayerController : ControllerBase
{
    private readonly IPlayerService _playerService;

    public PlayerController(IPlayerService playerService)
    {
        _playerService = playerService;
    }

    /// <summary>
    /// Retrieves a specific player by their ID
    /// </summary>
    /// <param name="id">The unique identifier of the player</param>
    /// <returns>The player details including personal information and current club</returns>
    /// <response code="200">Returns the requested player</response>
    /// <response code="404">If the player is not found</response>
    /// <response code="500">If there's an unexpected error</response>
    [HttpGet("{id}")]
    [ProducesResponseType(type: typeof(PlayerResponseDto), StatusCodes.Status200OK)]
    [Authorize]
    public async Task<IActionResult> GetPlayerById(int id)
    {
        try
        {
            var player = await _playerService.GetPlayerById(id);
            return Ok(player);
        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine($"Player not found: {ex}");
            return NotFound(new ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting player by id: {ex}");
            return StatusCode(500, new ErrorResponse(
                "An error occurred while processing your request",
                ex.Message));
        }
    }

    /// <summary>
    /// Releases a player from their current club
    /// </summary>
    /// <param name="id">The unique identifier of the player to release</param>
    /// <returns>No content on successful release</returns>
    /// <response code="204">If the player was successfully released</response>
    /// <response code="404">If the player is not found</response>
    /// <response code="500">If there's an unexpected error</response>
    [HttpPatch("{id}/release")]
    [ProducesResponseType(type: typeof(PlayerResponseDto), StatusCodes.Status200OK)]
    [Authorize]
    public async Task<IActionResult> ReleasePlayer(int id)
    {
        try
        {
            var player = await _playerService.ReleasePlayer(id);
            return Ok(player);
        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine($"Player not found: {ex}");
            return NotFound(new ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error releasing player: {ex}");
            return StatusCode(500, new ErrorResponse(
                "An error occurred while processing your request",
                ex.Message));
        }
    }

    /// <summary>
    /// Transfers a player from their current club to a new club
    /// </summary>
    /// <param name="id">The unique identifier of the player to transfer</param>
    /// <param name="clubId">The unique identifier of the destination club</param>
    /// <returns>No content on successful transfer</returns>
    /// <response code="204">If the player was successfully transferred</response>
    /// <response code="404">If the player or club is not found</response>
    /// <response code="500">If there's an unexpected error</response>
    [HttpPatch("{id}/transfer/{clubId}")]
    [ProducesResponseType(type: typeof(PlayerResponseDto), StatusCodes.Status200OK)]
    [Authorize]
    public async Task<IActionResult> TransferPlayer(int id, int clubId)
    {
        try
        {
            var player = await _playerService.TransferPlayer(id, clubId);
            return Ok(player);
        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine($"Resource not found: {ex}");
            return NotFound(new ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error transferring player: {ex}");
            return StatusCode(500, new ErrorResponse(
                "An error occurred while processing your request",
                ex.Message));
        }
    }

    /// <summary>
    /// Creates a new player in the system
    /// </summary>
    /// <param name="request">The player details including personal information, attributes, and club assignment</param>
    /// <returns>The newly created player with their assigned ID</returns>
    /// <response code="201">Returns the newly created player</response>
    /// <response code="404">If the specified club is not found</response>
    /// <response code="500">If there's an unexpected error</response>
    [HttpPost]
    [ProducesResponseType(type: typeof(PlayerResponseDto), StatusCodes.Status201Created)]
    [Authorize]
    public async Task<IActionResult> AddPlayer(CreatePlayerDto request)
    {
        try
        {
            var newPlayer = await _playerService.AddPlayer(request);
            return CreatedAtAction(nameof(GetPlayerById), new { id = newPlayer.Id }, newPlayer);
        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine($"Club not found: {ex}");
            return NotFound(new ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding player: {ex}");
            return StatusCode(500, new ErrorResponse(
                "An error occurred while processing your request",
                ex.Message));
        }
    }
}