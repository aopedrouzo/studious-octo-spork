using FootballManager.Application.DTOs;
using FootballManager.Application.DTOs.Request;
using FootballManager.Domain.Entities;

namespace FootballManager.Application.Mappers
{
    public static class CoachMapper
    {
        public static Coach ToEntity(this CreateCoachDto request)
        {
            return new Coach(
                request.FirstName,
                request.LastName,
                request.DateOfBirth,
                request.Salary,
                request.Email
            );
        }

        public static CoachResponseDto ToDto(this Coach entity)
        {
            return new CoachResponseDto
            {
                Id = entity.Id,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                DateOfBirth = entity.DateOfBirth,
                Salary = entity.Salary,
                Email = entity.Email
            };
        }
    }
}
