namespace SmartGas.Api.DTOs;

public class ProfileResponse
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string? BusinessName { get; set; }

    public string? Phone { get; set; }

    public string? District { get; set; }

    public string? Address { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}