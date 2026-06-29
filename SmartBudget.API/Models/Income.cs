namespace SmartBudget.API.Models;

public class Income
{
    public Guid IncomeId { get; set; }
    public Guid CycleId { get; set; }
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public string Source { get; set; } = string.Empty;
    public DateOnly IncomeDate { get; set; }
    public string? Notes { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public BudgetCycle Cycle { get; set; } = null!;
}