namespace SmartGas.Api.DTOs;

public class SettingResponse
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public string Language { get; set; } = string.Empty;

    public bool DarkMode { get; set; }

    public bool NotificationsEnabled { get; set; }

    public decimal GasThreshold { get; set; }

    public decimal TemperatureThreshold { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}