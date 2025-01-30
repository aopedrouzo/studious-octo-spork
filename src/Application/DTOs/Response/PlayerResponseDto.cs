using FootballManager.Domain.Enums;

public class PlayerResponseDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public Position Position { get; set; }
    public decimal Salary { get; set; }
    public string Email { get; set; } = string.Empty;
}