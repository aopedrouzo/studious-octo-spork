namespace FootballManager.Application.DTOs
{
    using System.ComponentModel.DataAnnotations;

    public class CreateClubDto
    {
        [Required(ErrorMessage = "Club name is required")]
        [StringLength(100, ErrorMessage = "Club name must be max 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Budget must be a positive number")]
        public decimal Budget { get; set; }
    }
}
