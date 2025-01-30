namespace FootballManager.Application.DTOs
{
    public class CoachResponseDto
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public decimal Salary { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}
