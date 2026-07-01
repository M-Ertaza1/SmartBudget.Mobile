namespace SmartBudget.API.DTOs.Budget;

public class CycleSummaryDto
{
    public Guid CycleId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal Balance { get; set; }
}