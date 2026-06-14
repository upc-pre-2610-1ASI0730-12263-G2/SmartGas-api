namespace SmartGas.Api.Models;

public class Zone
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Status { get; set; } = "Safe";

    public string Sensitivity { get; set; } = "Medium";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Account Account { get; set; } = null!;
}