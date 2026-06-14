namespace SmartGas.Api.Models;

public class Subscription
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public int PlanId { get; set; }

    public string Status { get; set; } = "Active";

    public DateTime StartDate { get; set; } = DateTime.UtcNow;

    public DateTime RenewalDate { get; set; } = DateTime.UtcNow.AddMonths(1);

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Account Account { get; set; } = null!;

    public Plan Plan { get; set; } = null!;
}