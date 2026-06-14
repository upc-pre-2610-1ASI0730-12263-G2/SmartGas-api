namespace SmartGas.Api.DTOs;

public class IncidentResponse
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public int IncidentId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public int ZoneId { get; set; }

    public string ZoneName { get; set; } = string.Empty;

    public int SensorId { get; set; }

    public string SensorCode { get; set; } = string.Empty;

    public string DetectedValue { get; set; } = string.Empty;

    public string Severity { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime DetectedAt { get; set; }

    public DateTime? ResolvedAt { get; set; }
}