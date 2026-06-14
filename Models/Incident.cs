namespace SmartGas.Api.Models;

public class Incident
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public int ZoneId { get; set; }

    public int SensorId { get; set; }

    public int? SensorReadingId { get; set; }

    public string Type { get; set; } = string.Empty;

    public string Severity { get; set; } = "Medium";

    public string Status { get; set; } = "Active";

    public DateTime DetectedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ReviewedAt { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Account Account { get; set; } = null!;

    public Zone Zone { get; set; } = null!;

    public Sensor Sensor { get; set; } = null!;

    public SensorReading? SensorReading { get; set; }
}