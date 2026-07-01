using Microsoft.EntityFrameworkCore;
using SmartBudget.API.Common;
using SmartBudget.API.Data;
using SmartBudget.API.DTOs.Budget;
using SmartBudget.API.Helpers;
using SmartBudget.API.Models;

namespace SmartBudget.API.Services;

public interface ICycleService
{
    Task<ApiResponse<CycleSummaryDto>> GetActiveCycleAsync(Guid userId);
}

public class CycleService : ICycleService
{
    private readonly AppDbContext _db;
    public CycleService(AppDbContext db) => _db = db;

    public async Task<ApiResponse<CycleSummaryDto>> GetActiveCycleAsync(Guid userId)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        if (user is null) return ApiResponse<CycleSummaryDto>.Fail("User not found.");

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var (start, end) = CycleCalculator.GetCycleDates(user.SalaryDay, today);

        // Find the cycle that covers today; create it if this is a new cycle period.
        var cycle = await _db.BudgetCycles.FirstOrDefaultAsync(c =>
            c.UserId == userId && c.StartDate == start && c.EndDate == end);

        if (cycle is null)
        {
            cycle = new BudgetCycle
            {
                CycleId = Guid.NewGuid(),
                UserId = userId,
                StartDate = start,
                EndDate = end,
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow
            };
            _db.BudgetCycles.Add(cycle);
            await _db.SaveChangesAsync();
        }

        var totalIncome = await _db.Income
            .Where(i => i.CycleId == cycle.CycleId)
            .SumAsync(i => (decimal?)i.Amount) ?? 0m;

        var totalExpenses = await _db.Expenses
            .Where(e => e.CycleId == cycle.CycleId)
            .SumAsync(e => (decimal?)e.Amount) ?? 0m;

        return ApiResponse<CycleSummaryDto>.Ok(new CycleSummaryDto
        {
            CycleId = cycle.CycleId,
            StartDate = cycle.StartDate,
            EndDate = cycle.EndDate,
            TotalIncome = totalIncome,
            TotalExpenses = totalExpenses,
            Balance = totalIncome - totalExpenses
        });
    }
}