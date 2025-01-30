using FootballManager.Application.DTOs;
using FootballManager.Application.Interfaces;
using FootballManager.Infrastructure.Notifications;

public class NotificationRouterService : INotificationService
{
    private readonly IServiceProvider _serviceProvider;

    public NotificationRouterService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task SendNotificationAsync(NotificationMessage message)
    {
        // Omits SMS and WhatsApp implementations for now
        INotificationService? service = message.Channel switch
        {
            NotificationChannel.Email => _serviceProvider.GetService<EmailNotificationService>(),
            NotificationChannel.SMS => throw new NotImplementedException(),
            NotificationChannel.WhatsApp => throw new NotImplementedException(),
            _ => throw new InvalidOperationException($"Unsupported channel: {message.Channel}")
        };

        if (service == null)
            throw new Exception($"No service registered for channel {message.Channel}");

        await service.SendNotificationAsync(message);
    }
}
