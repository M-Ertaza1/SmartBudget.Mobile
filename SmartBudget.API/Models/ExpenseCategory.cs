namespace SmartBudget.API.Models;

public class ExpenseCategory
{
    public Guid CategoryId { get; set; }
    public Guid? UserId { get; set; }     // null = system category
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public bool IsSystem { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; }

    public List<Expense> Expenses { get; set; } = new();
}

