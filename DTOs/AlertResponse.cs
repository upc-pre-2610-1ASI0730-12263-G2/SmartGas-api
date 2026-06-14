namespace SmartGas.Api.DTOs;

public class AlertResponse
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public int IncidentId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public string Severity { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? ResolvedAt { get; set; }
}