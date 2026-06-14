namespace SmartGas.Api.Models;

public class Notification
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public int? IncidentId { get; set; }

    public int? AlertId { get; set; }

    public string Channel { get; set; } = "Web";

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public bool IsRead { get; set; } = false;

    public bool IsConfirmed { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ReadAt { get; set; }

    public DateTime? ConfirmedAt { get; set; }

    public Account Account { get; set; } = null!;

    public Incident? Incident { get; set; }

    public Alert? Alert { get; set; }
}