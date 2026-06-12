namespace SmartGas.Api.Models;

public class SensorReading
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public int ZoneId { get; set; }

    public int SensorId { get; set; }

    public decimal? GasLevel { get; set; }

    public decimal? Temperature { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Account Account { get; set; } = null!;

    public Zone Zone { get; set; } = null!;

    public Sensor Sensor { get; set; } = null!;
}