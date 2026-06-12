namespace SmartGas.Api.DTOs;

public class ZoneResponse
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Status { get; set; } = string.Empty;

    public string Sensitivity { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}