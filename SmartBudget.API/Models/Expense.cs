
namespace SmartBudget.API.Models;

public class Expense
{
    public Guid ExpenseId { get; set; }
    public Guid CycleId { get; set; }
    public Guid UserId { get; set; }
    public Guid CategoryId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public DateOnly ExpenseDate { get; set; }
    public string? ReceiptUrl { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public BudgetCycle Cycle { get; set; } = null!;
    public ExpenseCategory Category { get; set; } = null!;
}