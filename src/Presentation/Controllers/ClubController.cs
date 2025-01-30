using FootballManager.Application.Common;
using FootballManager.Application.DTOs;
using FootballManager.Application.DTOs.Request;
using FootballManager.Application.DTOs.Response;
using FootballManager.Application.Interfaces;
using FootballManager.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballManager.Presentation.Controllers;

[Route("club")]
[ApiController]
public class ClubController : ControllerBase
{
    private readonly IClubService _clubService;

    public ClubController(IClubService clubService)
    {
        _clubService = clubService;
    }

    /// <summary>
    /// Retrieves all football clubs from the database
    /// </summary>
    /// <returns>A list of all clubs with their basic details including ID and name</returns>
    /// <response code="200">Returns the list of clubs</response>
    /// <response code="400">If there's an invalid operation</response>
    /// <response code="500">If there's an unexpected error</response>
    [HttpGet("all")]
    [ProducesResponseType(typeof(ClubListResponseDto), StatusCodes.Status200OK)]
    [Authorize]
    public async Task<IActionResult> GetAllClubs()
    {
        try
        {
            var clubs = await _clubService.GetAllClubs();
            return Ok(clubs);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Invalid operation: {ex}");
            return BadRequest(new ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting all clubs: {ex}");
            return StatusCode(500, new ErrorResponse(
                "An error occurred while processing your request",
                ex.Message));
        }
    }

    /// <summary>
    /// Retrieves a specific club by its ID
    /// </summary>
    /// <param name="id">The unique identifier of the club</param>
    /// <returns>The club details including name, budget, players, and coaches</returns>
    /// <response code="200">Returns the requested club</response>
    /// <response code="400">If the club ID is invalid</response>
    /// <response code="500">If there's an unexpected error</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ClubResponseDto), StatusCodes.Status200OK)]
    [Authorize]
    public async Task<IActionResult> GetClubById(int id)
    {
        try
        {
            var club = await _clubService.GetClubById(id);
            return Ok(club);
        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine($"Club not found: {ex}");
            return NotFound(new ErrorResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Invalid operation: {ex}");
            return BadRequest(new ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting club by id: {ex}");
            return StatusCode(500, new ErrorResponse(
                "An error occurred while processing your request",
                ex.Message));
        }
    }

    /// <summary>
    /// Creates a new football club
    /// </summary>
    /// <param name="request">The club details including name and initial budget</param>
    /// <returns>The newly created club with its assigned ID</returns>
    /// <response code="201">Returns the newly created club</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="500">If there's an unexpected error</response>
    [HttpPost]
    [ProducesResponseType(type: typeof(ClubResponseDto), StatusCodes.Status201Created)]
    [Authorize]
    public async Task<IActionResult> CreateClub([FromBody] CreateClubDto request)
    {
        try
        {
            var club = await _clubService.CreateClub(request);
            return CreatedAtAction(nameof(GetClubById), new { id = club.Id }, club);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Invalid operation: {ex}");
            return BadRequest(new ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating club: {ex}");
            return StatusCode(500, new ErrorResponse(
                "An error occurred while processing your request",
                ex.Message));
        }
    }

    /// <summary>
    /// Adds multiple players to an existing club
    /// </summary>
    /// <param name="id">The unique identifier of the club</param>
    /// <param name="request">List of players to be added to the club</param>
    /// <returns>The updated club with the new players added</returns>
    /// <response code="200">Returns the updated club</response>
    /// <response code="400">If the club ID or player data is invalid</response>
    /// <response code="500">If there's an unexpected error</response>
    [HttpPost("{id}/players")]
    [ProducesResponseType(type: typeof(ClubResponseDto), StatusCodes.Status201Created)]
    [Authorize]
    public async Task<IActionResult> AddPlayersToClub(int id, [FromBody] AddPlayersToClubRequest request)
    {
        try
        {
            var club = await _clubService.AddPlayersToClub(id, request);
            return CreatedAtAction(actionName: nameof(GetClubPlayers), new { id = club.Id }, club);
        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine($"Club not found: {ex}");
            return NotFound(new ErrorResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Invalid operation: {ex}");
            return BadRequest(new ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding players to club: {ex}");
            return StatusCode(500, new ErrorResponse(
                "An error occurred while processing your request",
                ex.Message));
        }
    }

    /// <summary>
    /// Adds a coach to an existing club
    /// </summary>
    /// <param name="id">The unique identifier of the club</param>
    /// <param name="request">The coach details to be added</param>
    /// <returns>The updated club with the new coach added</returns>
    /// <response code="200">Returns the updated club</response>
    /// <response code="400">If the club ID or coach data is invalid</response>
    /// <response code="500">If there's an unexpected error</response>
    [HttpPost("{id}/coach")]
    [ProducesResponseType(type: typeof(AddCoachToClubResponseDto), StatusCodes.Status201Created)]
    [Authorize]
    public async Task<IActionResult> AddCoachToClub(int id, [FromBody] CreateCoachDto request)
    {
        try
        {
            var response = await _clubService.AddCoachToClub(id, request);
            return CreatedAtAction(
                actionName: "GetCoachById",
                controllerName: "Coach",
                routeValues: new { id = response.Coach.Id },
                value: response);

        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine($"Club not found: {ex}");
            return NotFound(new ErrorResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Invalid operation: {ex}");
            return BadRequest(new ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding coach to club: {ex}");
            return StatusCode(500, new ErrorResponse(
                "An error occurred while processing your request",
                ex.Message));
        }
    }

    /// <summary>
    /// Updates the budget of an existing club
    /// </summary>
    /// <param name="id">The unique identifier of the club</param>
    /// <param name="request">The amount to add (positive) or subtract (negative) from the current budget</param>
    /// <returns>The updated club with the modified budget</returns>
    /// <response code="200">Returns the updated club</response>
    /// <response code="400">If the club ID or budget amount is invalid</response>
    /// <response code="500">If there's an unexpected error</response>
    [HttpPatch("{id}/budget")]
    [ProducesResponseType(type: typeof(ClubResponseDto), StatusCodes.Status200OK)]
    [Authorize]
    public async Task<IActionResult> AdjustClubBudget(int id, [FromBody] UpdateClubBudgetRequest request)
    {
        try
        {
            var club = await _clubService.AdjustClubBudget(id, request);
            return Ok(club);
        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine($"Club not found: {ex}");
            return NotFound(new ErrorResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Invalid operation: {ex}");
            return BadRequest(new ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating club budget: {ex}");
            return StatusCode(500, new ErrorResponse(
                "An error occurred while processing your request",
                ex.Message));
        }
    }

    /// <summary>
    /// Retrieves players from a specific club with optional filtering and pagination
    /// </summary>
    /// <param name="id">The unique identifier of the club</param>
    /// <param name="name">Optional filter by player name</param>
    /// <param name="position">Optional filter by player position</param>
    /// <param name="minSalary">Optional filter by minimum salary</param>
    /// <param name="maxSalary">Optional filter by maximum salary</param>
    /// <param name="paginationParams">Optional pagination parameters</param>
    /// <returns>A paginated list of players matching the filter criteria</returns>
    /// <response code="200">Returns the filtered list of players</response>
    /// <response code="400">If any filter parameters are invalid</response>
    /// <response code="500">If there's an unexpected error</response>
    [HttpGet("{id}/players")]
    [ProducesResponseType(typeof(PaginatedResult<PlayerResponseDto>), StatusCodes.Status200OK)]
    [Authorize]
    public async Task<IActionResult> GetClubPlayers(
        int id,
        [FromQuery] string? name = null,
        [FromQuery] Position? position = null,
        [FromQuery] decimal? minSalary = null,
        [FromQuery] decimal? maxSalary = null,
        [FromQuery] PaginationParams? paginationParams = null)
    {
        try
        {
            var players = await _clubService.GetClubPlayers(id, name, position, minSalary, maxSalary, paginationParams);
            return Ok(players);
        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine($"Club not found: {ex}");
            return NotFound(new ErrorResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Invalid operation: {ex}");
            return BadRequest(new ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting club players: {ex}");
            return StatusCode(500, new ErrorResponse(
                "An error occurred while processing your request",
                ex.Message));
        }
    }
}