using Microsoft.EntityFrameworkCore;
using SmartBudget.API.Common;
using SmartBudget.API.Data;
using SmartBudget.API.DTOs.Budget;
using SmartBudget.API.Helpers;
using SmartBudget.API.Models;

namespace SmartBudget.API.Services;

public interface IExpenseService
{
    Task<ApiResponse<List<CategoryDto>>> GetCategoriesAsync(Guid userId);
    Task<ApiResponse<List<ExpenseDto>>> GetExpensesAsync(Guid userId);
    Task<ApiResponse<ExpenseDto>> CreateExpenseAsync(Guid userId, CreateExpenseDto dto);
}

public class ExpenseService : IExpenseService
{
    private readonly AppDbContext _db;
    public ExpenseService(AppDbContext db) => _db = db;

    public async Task<ApiResponse<List<CategoryDto>>> GetCategoriesAsync(Guid userId)
    {
        // System categories (user_id null) + this user's custom ones.
        var cats = await _db.ExpenseCategories
            .Where(c => c.IsActive && (c.IsSystem || c.UserId == userId))
            .OrderByDescending(c => c.IsSystem)
            .ThenBy(c => c.Name)
            .Select(c => new CategoryDto
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                Color = c.Color ?? "#BDC3C7"
            })
            .ToListAsync();

        return ApiResponse<List<CategoryDto>>.Ok(cats);
    }

    public async Task<ApiResponse<List<ExpenseDto>>> GetExpensesAsync(Guid userId)
    {
        var cycle = await GetActiveCycleAsync(userId);
        if (cycle is null) return ApiResponse<List<ExpenseDto>>.Ok(new());

        var items = await _db.Expenses
            .Where(e => e.CycleId == cycle.CycleId)
            .OrderByDescending(e => e.ExpenseDate)
            .Select(e => new ExpenseDto
            {
                ExpenseId = e.ExpenseId,
                CategoryId = e.CategoryId,
                CategoryName = e.Category.Name,
                CategoryColor = e.Category.Color ?? "#BDC3C7",
                Amount = e.Amount,
                ExpenseDate = e.ExpenseDate,
                Description = e.Description
            })
            .ToListAsync();

        return ApiResponse<List<ExpenseDto>>.Ok(items);
    }

    public async Task<ApiResponse<ExpenseDto>> CreateExpenseAsync(Guid userId, CreateExpenseDto dto)
    {
        if (dto.Amount <= 0)
            return ApiResponse<ExpenseDto>.Fail("Amount must be greater than zero.");

        var category = await _db.ExpenseCategories.FirstOrDefaultAsync(c =>
            c.CategoryId == dto.CategoryId && (c.IsSystem || c.UserId == userId));
        if (category is null)
            return ApiResponse<ExpenseDto>.Fail("Invalid category.");

        var cycle = await GetActiveCycleAsync(userId);
        if (cycle is null) return ApiResponse<ExpenseDto>.Fail("No active budget cycle.");

        var now = DateTimeOffset.UtcNow;
        var expense = new Expense
        {
            ExpenseId = Guid.NewGuid(),
            CycleId = cycle.CycleId,
            UserId = userId,
            CategoryId = category.CategoryId,
            Amount = dto.Amount,
            ExpenseDate = dto.ExpenseDate,
            Description = dto.Description,
            CreatedAt = now,
            UpdatedAt = now
        };

        _db.Expenses.Add(expense);
        await _db.SaveChangesAsync();

        return ApiResponse<ExpenseDto>.Ok(new ExpenseDto
        {
            ExpenseId = expense.ExpenseId,
            CategoryId = category.CategoryId,
            CategoryName = category.Name,
            CategoryColor = category.Color ?? "#BDC3C7",
            Amount = expense.Amount,
            ExpenseDate = expense.ExpenseDate,
            Description = expense.Description
        });
    }

    // Shared helper: find (or create) the cycle covering today for this user.
    private async Task<BudgetCycle?> GetActiveCycleAsync(Guid userId)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        if (user is null) return null;

        var (start, end) = CycleCalculator.GetCycleDates(
            user.SalaryDay, DateOnly.FromDateTime(DateTime.UtcNow));

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
        return cycle;
    }
}