namespace SmartBudget.Mobile.Models;

public class CycleSummary
{
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }

    // Phase 4 formula (simplified for now): balance = income - expenses
    public decimal Balance => TotalIncome - TotalExpenses;
}