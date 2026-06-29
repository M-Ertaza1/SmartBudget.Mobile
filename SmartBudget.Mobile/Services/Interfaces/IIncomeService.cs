using SmartBudget.Mobile.Models;

namespace SmartBudget.Mobile.Services.Interfaces;

public interface IIncomeService
{
    Task<IReadOnlyList<Income>> GetIncomeAsync();
    Task<ApiResponse<Income>> AddIncomeAsync(CreateIncomeRequest request);
    Task<decimal> GetTotalAsync();
}