namespace SmartGas.Api.DTOs;

public class CreateZoneRequest
{
    public int AccountId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Sensitivity { get; set; } = "Medium";
}