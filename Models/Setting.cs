namespace SmartGas.Api.Models;

public class Setting
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public string Language { get; set; } = "en-US";

    public bool DarkMode { get; set; } = false;

    public bool NotificationsEnabled { get; set; } = true;

    public decimal GasThreshold { get; set; } = 50;

    public decimal TemperatureThreshold { get; set; } = 45;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Account Account { get; set; } = null!;
}