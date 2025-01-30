using FootballManager.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FootballManager.Application.DTOs.Request;

public class CreatePlayerDto
{
    [Required(ErrorMessage = "First name is required")]
    [StringLength(50, ErrorMessage = "First name must be max 50 characters")]
    public required string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50, ErrorMessage = "Last name must be max 50 characters")]
    public required string LastName { get; set; }

    [Required(ErrorMessage = "Date of birth is required")]
    public required DateOnly DateOfBirth { get; set; }

    [Required(ErrorMessage = "Position is required")]
    public required Position Position { get; set; }

    [Required(ErrorMessage = "Salary is required")]
    [Range(0, double.MaxValue, ErrorMessage = "Salary must be a positive number")]
    public required decimal Salary { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    public required string Email { get; set; }
}
