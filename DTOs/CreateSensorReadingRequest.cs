namespace SmartGas.Api.DTOs;

public class CreateSensorReadingRequest
{
    public string SensorCode { get; set; } = string.Empty;

    public decimal? GasLevel { get; set; }

    public decimal? Temperature { get; set; }
}