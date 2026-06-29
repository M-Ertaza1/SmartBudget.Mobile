using SmartBudget.Mobile.Models;
using SmartBudget.Mobile.Services.Interfaces;

namespace SmartBudget.Mobile.Services;

// In-memory store, Singleton so the list is shared app-wide. Resets on restart.
public class MockIncomeService : IIncomeService
{
    private readonly List<Income> _income = new();

    public Task<IReadOnlyList<Income>> GetIncomeAsync()
        => Task.FromResult<IReadOnlyList<Income>>(
            _income.OrderByDescending(i => i.IncomeDate).ToList());

    public async Task<ApiResponse<Income>> AddIncomeAsync(CreateIncomeRequest request)
    {
        await Task.Delay(400); // simulate the API call

        if (request.Amount <= 0)
            return new ApiResponse<Income> { Success = false, Error = "Amount must be greater than zero." };

        if (string.IsNullOrWhiteSpace(request.Source))
            return new ApiResponse<Income> { Success = false, Error = "Please enter an income source." };

        var income = new Income
        {
            IncomeId = Guid.NewGuid(),
            Amount = request.Amount,
            Source = request.Source,
            IncomeDate = request.IncomeDate,
            Notes = request.Notes
        };

        _income.Add(income);
        return new ApiResponse<Income> { Success = true, Data = income };
    }

    public Task<decimal> GetTotalAsync()
        => Task.FromResult(_income.Sum(i => i.Amount));
}