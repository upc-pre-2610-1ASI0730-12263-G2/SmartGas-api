namespace SmartGas.Api.Models;

public class Sensor
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public int ZoneId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = "MultiSensor";

    public string Status { get; set; } = "Online";

    public int BatteryLevel { get; set; } = 100;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Account Account { get; set; } = null!;

    public Zone Zone { get; set; } = null!;
}