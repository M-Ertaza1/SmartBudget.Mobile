using SmartBudget.Mobile.Models;
using SmartBudget.Mobile.Services.Interfaces;

namespace SmartBudget.Mobile.Services;

// In-memory store: data persists while the app runs, resets on restart.
// Registered as a Singleton so the same list is shared across the app.
public class MockExpenseService : IExpenseService
{
    private readonly List<Expense> _expenses = new();
    private readonly List<ExpenseCategory> _categories;

    public MockExpenseService()
    {
        // The 10 system categories seeded in Phase 3.
        _categories = new()
        {
            new() { CategoryId = Guid.NewGuid(), Name = "Food & Dining",  Color = "#FF6B35" },
            new() { CategoryId = Guid.NewGuid(), Name = "Transport",      Color = "#4ECDC4" },
            new() { CategoryId = Guid.NewGuid(), Name = "Utilities",      Color = "#45B7D1" },
            new() { CategoryId = Guid.NewGuid(), Name = "Healthcare",     Color = "#96CEB4" },
            new() { CategoryId = Guid.NewGuid(), Name = "Entertainment",  Color = "#FFEAA7" },
            new() { CategoryId = Guid.NewGuid(), Name = "Education",      Color = "#DDA0DD" },
            new() { CategoryId = Guid.NewGuid(), Name = "Clothing",       Color = "#F0A500" },
            new() { CategoryId = Guid.NewGuid(), Name = "Personal Care",  Color = "#A8E6CF" },
            new() { CategoryId = Guid.NewGuid(), Name = "Savings",        Color = "#88D8B0" },
            new() { CategoryId = Guid.NewGuid(), Name = "Other",          Color = "#BDC3C7" },
        };
    }

    public Task<IReadOnlyList<ExpenseCategory>> GetCategoriesAsync()
        => Task.FromResult<IReadOnlyList<ExpenseCategory>>(_categories);

    public Task<IReadOnlyList<Expense>> GetExpensesAsync()
        => Task.FromResult<IReadOnlyList<Expense>>(
            _expenses.OrderByDescending(e => e.ExpenseDate).ToList());

    public async Task<ApiResponse<Expense>> AddExpenseAsync(CreateExpenseRequest request)
    {
        await Task.Delay(400); // simulate the API call

        if (request.Amount <= 0)
            return new ApiResponse<Expense> { Success = false, Error = "Amount must be greater than zero." };

        var category = _categories.FirstOrDefault(c => c.CategoryId == request.CategoryId);
        if (category is null)
            return new ApiResponse<Expense> { Success = false, Error = "Please choose a category." };

        var expense = new Expense
        {
            ExpenseId = Guid.NewGuid(),
            CategoryId = category.CategoryId,
            CategoryName = category.Name,
            CategoryColor = category.Color,
            Amount = request.Amount,
            ExpenseDate = request.ExpenseDate,
            Description = request.Description
        };

        _expenses.Add(expense);
        return new ApiResponse<Expense> { Success = true, Data = expense };
    }

    public Task<decimal> GetTotalAsync()
        => Task.FromResult(_expenses.Sum(e => e.Amount));
}