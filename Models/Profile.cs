namespace SmartGas.Api.Models;

public class Profile
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string? BusinessName { get; set; }

    public string? Phone { get; set; }

    public string? District { get; set; }

    public string? Address { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Account Account { get; set; } = null!;
}