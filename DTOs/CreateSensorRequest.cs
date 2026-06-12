namespace SmartGas.Api.DTOs;

public class CreateSensorRequest
{
    public int AccountId { get; set; }

    public int ZoneId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = "MultiSensor";
}