using FootballManager.Application.Common;
using FootballManager.Application.DTOs;
using FootballManager.Application.DTOs.Request;
using FootballManager.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballManager.Presentation.Controllers;

[Route("coach")]
[ApiController]
public class CoachController : ControllerBase
{
    private readonly ICoachService _coachService;

    public CoachController(ICoachService coachService)
    {
        _coachService = coachService;
    }

    /// <summary>
    /// Retrieves a specific coach by their ID
    /// </summary>
    /// <param name="id">The unique identifier of the coach</param>
    /// <returns>The coach details including personal information and current club</returns>
    /// <response code="200">Returns the requested coach</response>
    /// <response code="404">If the coach is not found</response>
    /// <response code="500">If there's an unexpected error</response>
    [HttpGet("{id}")]
    [ProducesResponseType(type: typeof(CoachResponseDto), StatusCodes.Status200OK)]
    [Authorize] 
    public async Task<IActionResult> GetCoachById(int id)
    {
        try
        {
            var coach = await _coachService.GetCoachById(id);
            return Ok(coach);
        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine($"Coach not found: {ex}");
            return NotFound(new ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting coach by id: {ex}");
            return StatusCode(500, new ErrorResponse(
                "An error occurred while processing your request",
                ex.Message));
        }
    }

    /// <summary>
    /// Creates a new coach in the system
    /// </summary>
    /// <param name="request">The coach details including name, date of birth, and salary</param>
    /// <returns>The newly created coach with their assigned ID</returns>
    /// <response code="201">Returns the newly created coach</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="500">If there's an unexpected error</response>
    [HttpPost]
    [ProducesResponseType(type: typeof(CoachResponseDto), StatusCodes.Status201Created)]
    [Authorize]
    public async Task<IActionResult> AddCoach([FromBody] CreateCoachDto request)
    {
        try
        {
            var newCoach = await _coachService.AddCoach(request);
            return CreatedAtAction(nameof(GetCoachById), new { id = newCoach.Id }, newCoach);
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine($"Invalid argument: {ex}");
            return BadRequest(new ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding coach: {ex}");
            return StatusCode(500, new ErrorResponse(
                "An error occurred while processing your request",
                ex.Message));
        }
    }



    /// <summary>
    /// Transfers a coach from their current club to a new club
    /// </summary>
    /// <param name="coachId">The unique identifier of the coach to transfer</param>
    /// <param name="clubId">The unique identifier of the destination club</param>
    /// <returns>No content on successful transfer</returns>
    /// <response code="204">If the coach was successfully transferred</response>
    /// <response code="404">If the coach or club is not found</response>
    /// <response code="500">If there's an unexpected error</response>
    [HttpPatch("{coachId}/transfer/{clubId}")]
    [ProducesResponseType(type: typeof(CoachResponseDto), StatusCodes.Status200OK)]
    [Authorize]
    public async Task<IActionResult> TransferCoach(int coachId, int clubId)
    {
        try
        {
            var coach = await _coachService.TransferCoach(coachId, clubId);
            return Ok(coach);
        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine($"Resource not found: {ex}");
            return NotFound(new ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error transferring coach: {ex}");
            return StatusCode(500, new ErrorResponse(
                "An error occurred while processing your request",
                ex.Message));
        }
    }

    /// <summary>
    /// Releases a coach from their current club
    /// </summary>
    /// <param name="coachId">The unique identifier of the coach to release</param>
    /// <returns>The released coach</returns>
    /// <response code="200">If the coach was successfully released</response>
    /// <response code="404">If the coach is not found</response>
    /// <response code="500">If there's an unexpected error</response>
    [HttpPatch("{coachId}/release")]
    [ProducesResponseType(type: typeof(CoachResponseDto), StatusCodes.Status200OK)]
    [Authorize]
    public async Task<IActionResult> ReleaseCoach(int coachId)
    {
        try
        {
            var coach = await _coachService.ReleaseCoach(coachId);
            return Ok(coach);
        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine($"Coach not found: {ex}");
            return NotFound(new ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error releasing coach: {ex}");
            return StatusCode(500, new ErrorResponse(
                "An error occurred while processing your request",
                ex.Message));
        }
    }
}