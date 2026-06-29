namespace SmartBudget.API.Models;

public class Subscription
{
    public Guid SubscriptionId { get; set; }
    public Guid UserId { get; set; }
    public string Plan { get; set; } = "free";
    public string Status { get; set; } = "active";
    public string? PaymentRef { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public User User { get; set; } = null!;
}