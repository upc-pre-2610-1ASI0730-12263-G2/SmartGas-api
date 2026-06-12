namespace SmartGas.Api.DTOs;

public class UpdateSensorRequest
{
    public int? ZoneId { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public string? Type { get; set; }

    public string? Status { get; set; }
}