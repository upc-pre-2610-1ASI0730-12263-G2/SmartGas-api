namespace SmartGas.Api.DTOs;

public class PlanResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int MaxZones { get; set; }

    public int MaxSensors { get; set; }

    public string Features { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}