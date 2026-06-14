namespace SmartGas.Api.DTOs;

public class DashboardSummaryResponse
{
    public int AccountId { get; set; }

    public int TotalZones { get; set; }

    public int TotalSensors { get; set; }

    public int TotalReadings { get; set; }

    public int ActiveIncidents { get; set; }

    public int CriticalIncidents { get; set; }

    public int UnreadNotifications { get; set; }

    public string CurrentPlan { get; set; } = string.Empty;

    public List<DashboardLatestReadingResponse> LatestReadings { get; set; } = new();

    public List<DashboardLatestIncidentResponse> LatestIncidents { get; set; } = new();
}

public class DashboardLatestReadingResponse
{
    public int Id { get; set; }

    public int SensorId { get; set; }

    public string SensorCode { get; set; } = string.Empty;

    public int ZoneId { get; set; }

    public string ZoneName { get; set; } = string.Empty;

    public decimal? GasLevel { get; set; }

    public decimal? Temperature { get; set; }

    public DateTime CreatedAt { get; set; }
}

public class DashboardLatestIncidentResponse
{
    public int Id { get; set; }

    public int SensorId { get; set; }

    public string SensorCode { get; set; } = string.Empty;

    public int ZoneId { get; set; }

    public string ZoneName { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string Severity { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime DetectedAt { get; set; }
}