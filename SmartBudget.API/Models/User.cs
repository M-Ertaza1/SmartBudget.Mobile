namespace SmartBudget.API.Models;

public class User
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? Mobile { get; set; }
    public int SalaryDay { get; set; }
    public bool IsEmailVerified { get; set; }
    public string? VerificationToken { get; set; }
    public string? ResetToken { get; set; }
    public DateTimeOffset? ResetExpires { get; set; }
    public string? AvatarUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    // Navigation
    public Subscription? Subscription { get; set; }
    public List<BudgetCycle> BudgetCycles { get; set; } = new();
}