using SmartBudget.Mobile.Models;

namespace SmartBudget.Mobile.Services.Interfaces;

public interface IExpenseService
{
    Task<IReadOnlyList<ExpenseCategory>> GetCategoriesAsync();
    Task<IReadOnlyList<Expense>> GetExpensesAsync();
    Task<ApiResponse<Expense>> AddExpenseAsync(CreateExpenseRequest request);
    Task<decimal> GetTotalAsync();
}