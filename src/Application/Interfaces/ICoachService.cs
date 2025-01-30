using FootballManager.Application.DTOs;
using FootballManager.Application.DTOs.Request;
using FootballManager.Domain.Entities;

namespace FootballManager.Application.Interfaces;

public interface ICoachService
{
    Task<CoachResponseDto> GetCoachById(int id);
    Task<CoachResponseDto> AddCoach(CreateCoachDto coach);
    Task<CoachResponseDto> TransferCoach(int coachId, int clubId);
    Task<CoachResponseDto> ReleaseCoach(int coachId);
    
}