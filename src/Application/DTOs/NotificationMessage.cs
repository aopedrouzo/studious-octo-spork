namespace FootballManager.Application.DTOs;

public class NotificationMessage
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string? RecipientId { get; set; }
    public Dictionary<string, string>? AdditionalData { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public NotificationChannel Channel { get; set; }
}



public enum NotificationChannel
{
    Email,
    SMS,
    WhatsApp
}