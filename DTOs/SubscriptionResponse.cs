namespace SmartGas.Api.DTOs;

public class SubscriptionResponse
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public int PlanId { get; set; }

    public string PlanName { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int MaxZones { get; set; }

    public int MaxSensors { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime RenewalDate { get; set; }
}