namespace SmartGas.Api.Models;

public class Account
{
    public int Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = "HomeOwner";

    public string Status { get; set; } = "Active";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Profile? Profile { get; set; }

    public Setting? Setting { get; set; }

    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

    public ICollection<Zone> Zones { get; set; } = new List<Zone>();

    public ICollection<Sensor> Sensors { get; set; } = new List<Sensor>();

    public ICollection<Incident> Incidents { get; set; } = new List<Incident>();

    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}