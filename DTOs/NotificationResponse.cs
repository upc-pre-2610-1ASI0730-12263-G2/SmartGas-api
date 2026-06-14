namespace SmartGas.Api.DTOs;

public class NotificationResponse
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public int? IncidentId { get; set; }

    public int? AlertId { get; set; }

    public string Channel { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public bool IsRead { get; set; }

    public bool IsConfirmed { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ReadAt { get; set; }

    public DateTime? ConfirmedAt { get; set; }
}