namespace SmartGas.Api.Models;

public class Alert
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public int IncidentId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public string Severity { get; set; } = "Medium";

    public string Status { get; set; } = "Active";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ResolvedAt { get; set; }

    public Account Account { get; set; } = null!;

    public Incident Incident { get; set; } = null!;
}