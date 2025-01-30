namespace FootballManager.Application.DTOs
{
    public class ClubListResponseDto
    {
        public IEnumerable<ClubListItemDto> Clubs { get; set; } = new List<ClubListItemDto>();
    }

    public class ClubListItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
