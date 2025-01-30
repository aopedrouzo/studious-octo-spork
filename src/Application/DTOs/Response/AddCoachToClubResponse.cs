namespace FootballManager.Application.DTOs.Response
{
    public class AddCoachToClubResponseDto
    {
        public required ClubResponseDto Club { get; set; }
        public required CoachResponseDto Coach { get; set; }
    }
}
