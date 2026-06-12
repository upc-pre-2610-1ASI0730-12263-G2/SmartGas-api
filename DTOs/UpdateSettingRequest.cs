namespace SmartGas.Api.DTOs;

public class UpdateSettingRequest
{
    public string Language { get; set; } = "en-US";

    public bool DarkMode { get; set; }

    public bool NotificationsEnabled { get; set; }

    public decimal GasThreshold { get; set; }

    public decimal TemperatureThreshold { get; set; }
}