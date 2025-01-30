using FootballManager.Application.DTOs;
using FootballManager.Application.DTOs.Request;
using FootballManager.Application.Interfaces;
using FootballManager.Application.Mappers;
using FootballManager.Domain.Entities;

namespace FootballManager.Application.Services;

public class CoachService : ICoachService
{
    private readonly ICoachRepository _coachRepository;
    private readonly IClubRepository _clubRepository;

    public CoachService(ICoachRepository coachRepository, IClubRepository clubRepository)
    {
        _coachRepository = coachRepository;
        _clubRepository = clubRepository;
    }

    public async Task<CoachResponseDto> GetCoachById(int id)
    {
        var coach = await _coachRepository.GetByIdAsync(id);
        if (coach == null)
        {
            throw new KeyNotFoundException($"Coach with ID {id} not found");
        }
        return coach.ToDto();
    }

    public async Task<CoachResponseDto> AddCoach(CreateCoachDto request)
    {
        var coach = request.ToEntity();

        var addedCoach = await _coachRepository.AddAsync(coach);
        return addedCoach.ToDto();
    }

    public async Task<CoachResponseDto> TransferCoach(int coachId, int clubId)
    {
        var coach = await _coachRepository.GetByIdAsync(coachId);
        if (coach == null)
            throw new KeyNotFoundException($"Coach with ID {coachId} not found");

        if (coach.ClubId != null)
        {
            var clubOrigin = await _clubRepository.GetByIdAsync(coach.ClubId.Value);
            if (clubOrigin != null)
            {
                clubOrigin.HandleRelease(coach);
                await _clubRepository.UpdateAsync(clubOrigin);
                coach.LeaveClub();
            }
        }

        var club = await _clubRepository.GetByIdAsync(clubId);
        if (club == null)
            throw new KeyNotFoundException($"Club with ID {clubId} not found");

        club.HandleTransfer(coach);
        await _clubRepository.UpdateAsync(club);

        coach.JoinClub(club);
        await _coachRepository.UpdateAsync(coach);
        return coach.ToDto();
    }

    public async Task<CoachResponseDto> ReleaseCoach(int coachId)
    {
        var coach = await _coachRepository.GetByIdAsync(coachId);
        if (coach == null)
            throw new KeyNotFoundException($"Coach with ID {coachId} not found");

        if (coach.ClubId != null)
        {
            var clubOrigin = await _clubRepository.GetByIdAsync(coach.ClubId.Value);
            if (clubOrigin != null)
            {
                clubOrigin.HandleRelease(coach);
                await _clubRepository.UpdateAsync(clubOrigin);
            }
        }

        coach.LeaveClub();
        await _coachRepository.UpdateAsync(coach);
        return coach.ToDto();
    }

}