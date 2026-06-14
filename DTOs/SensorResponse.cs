namespace SmartGas.Api.DTOs;

public class SensorResponse
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public int ZoneId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public int BatteryLevel { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}