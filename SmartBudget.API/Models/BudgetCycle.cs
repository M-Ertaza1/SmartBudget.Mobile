namespace SmartBudget.API.Models;

public class BudgetCycle
{
    public Guid CycleId { get; set; }
    public Guid UserId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public User User { get; set; } = null!;
    public List<Income> Incomes { get; set; } = new();
    public List<Expense> Expenses { get; set; } = new();
}