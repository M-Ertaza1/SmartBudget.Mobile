namespace SmartBudget.API.DTOs.Budget;

public class ExpenseDto
{
    public Guid ExpenseId { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryColor { get; set; } = "#BDC3C7";
    public decimal Amount { get; set; }
    public DateOnly ExpenseDate { get; set; }
    public string? Description { get; set; }
}

public class CategoryDto
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#BDC3C7";
}

public class CreateExpenseDto
{
    public decimal Amount { get; set; }
    public Guid CategoryId { get; set; }
    public DateOnly ExpenseDate { get; set; }
    public string? Description { get; set; }
}