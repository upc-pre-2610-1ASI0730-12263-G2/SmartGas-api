namespace SmartGas.Api.DTOs;

public class AuthResponse
{
    public int AccountId { get; set; }

    public string Email { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public int PlanId { get; set; }

    public string PlanName { get; set; } = string.Empty;
}