namespace SmartGas.Api.DTOs;

public class SensorReadingResponse
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public int ZoneId { get; set; }

    public int SensorId { get; set; }

    public decimal? GasLevel { get; set; }

    public decimal? Temperature { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IncidentCreated { get; set; }

    public int? IncidentId { get; set; }

    public string? IncidentType { get; set; }

    public string? Severity { get; set; }

    public string SensorStatus { get; set; } = string.Empty;

    public string ZoneStatus { get; set; } = string.Empty;
}