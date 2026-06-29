namespace SmartBudget.Mobile.Models;

public class Income
{
    public Guid IncomeId { get; set; }
    public decimal Amount { get; set; }
    public string Source { get; set; } = string.Empty;
    public DateOnly IncomeDate { get; set; }
    public string? Notes { get; set; }
}

// Mirrors the API's CreateIncomeDto (Phase 4)
public class CreateIncomeRequest
{
    public decimal Amount { get; set; }
    public string Source { get; set; } = string.Empty;
    public DateOnly IncomeDate { get; set; }
    public string? Notes { get; set; }
}