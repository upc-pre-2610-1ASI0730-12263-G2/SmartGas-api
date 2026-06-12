namespace SmartGas.Api.DTOs;

public class EmergencyContactResponse
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}

public class UpdateEmergencyContactRequest
{
    public string Name { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
}