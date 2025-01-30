using FootballManager.Application.DTOs;
using FootballManager.Application.DTOs.Request;
using FootballManager.Domain.Entities;

namespace FootballManager.Application.Mappers;

public static class PlayerMapper
{
    public static Player ToEntity(this CreatePlayerDto dto)
    {
        return new Player(
            firstName: dto.FirstName,
            lastName: dto.LastName,
            dateOfBirth: dto.DateOfBirth,
            position: dto.Position,
            salary: dto.Salary,
            email: dto.Email
        );
    }

    public static IEnumerable<Player> ToEntities(this AddPlayersToClubRequest request)
    {
        return request.Players.Select(dto => dto.ToEntity());
    }

    public static PlayerResponseDto ToDto(this Player entity)
    {
        return new PlayerResponseDto
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            DateOfBirth = entity.DateOfBirth,
            Position = entity.Position,
            Salary = entity.Salary,
            Email = entity.Email
        };
    }
}
