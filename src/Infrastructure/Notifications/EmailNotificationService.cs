using FootballManager.Application.DTOs;
using FootballManager.Application.Interfaces;

namespace FootballManager.Infrastructure.Notifications;

public class EmailNotificationService : INotificationService
{
    public async Task SendNotificationAsync(NotificationMessage message)
    {
         // Simulate the delay of sending a message
        await Task.Delay(500); // Simulate real-world latency

        // Write mock to the console as I don't have a real email service available
        Console.WriteLine("=== MOCK NOTIFICATION SERVICE ===");
        Console.WriteLine($"To: {message.RecipientId}");
        Console.WriteLine($"Subject: {message.Title}");
        Console.WriteLine($"Body: {message.Body}");
        Console.WriteLine($"Channel: {message.Channel}");
        Console.WriteLine("=================================");
    }
}