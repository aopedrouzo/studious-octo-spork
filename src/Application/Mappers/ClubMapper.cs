using FootballManager.Application.DTOs;
using FootballManager.Domain.Entities;
using FootballManager.Application.DTOs.Response;
using FootballManager.Application.Common;

namespace FootballManager.Application.Mappers
{
    public static class ClubMapper
    {
        public static Club ToEntity(this CreateClubDto request)
        {
            return new Club(
                name: request.Name,
                budget: request.Budget
            );
        }
        

        public static ClubListResponseDto ToListResponse(this IEnumerable<Club> clubs)
        {
            return new ClubListResponseDto
            {
                Clubs = clubs.Select(club => new ClubListItemDto
                {
                    Name = club.Name,
                    Id = club.Id
                }).ToList()
            };
        }

        public static ClubResponseDto ToDto(this Club club)
        {
            return new ClubResponseDto
            {
                Id = club.Id,
                Name = club.Name,
                Budget = club.Budget
            };
        }

        public static AddCoachToClubResponseDto ToAddCoachResponse(this Club club, Coach coach)
        {
            return new AddCoachToClubResponseDto
            {
                Club = club.ToDto(),
                Coach = new CoachResponseDto
                {
                    Id = coach.Id,
                    FirstName = coach.FirstName,
                    LastName = coach.LastName,
                    DateOfBirth = coach.DateOfBirth,
                    Salary = coach.Salary,
                    Email = coach.Email,
                }
            };
        }

        public static PaginatedResult<PlayerResponseDto> ToDto(this PaginatedResult<Player> paginatedPlayers)
        {
            var playerDtos = paginatedPlayers.Items.Select(player => player.ToDto());
            
            return new PaginatedResult<PlayerResponseDto>(
                items: playerDtos,
                totalCount: paginatedPlayers.TotalCount,
                pageNumber: paginatedPlayers.PageNumber,
                pageSize: (paginatedPlayers.Items.Count() > 0) 
                    ? (int)Math.Ceiling(paginatedPlayers.TotalCount / (double)paginatedPlayers.TotalPages) 
                    : 10
            );
        }
    }
}
