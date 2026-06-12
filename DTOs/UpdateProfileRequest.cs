namespace SmartGas.Api.DTOs;

public class UpdateProfileRequest
{
    public string FullName { get; set; } = string.Empty;

    public string? BusinessName { get; set; }

    public string? Phone { get; set; }

    public string? District { get; set; }

    public string? Address { get; set; }
}