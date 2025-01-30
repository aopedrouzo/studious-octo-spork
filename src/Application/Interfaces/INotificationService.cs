using FootballManager.Application.DTOs;

namespace FootballManager.Application.Interfaces;

public interface INotificationService
{
    Task SendNotificationAsync(NotificationMessage message);
}
