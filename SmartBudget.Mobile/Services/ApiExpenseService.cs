using System.Net.Http.Json;
using System.Text.Json;
using SmartBudget.Mobile.Models;
using SmartBudget.Mobile.Services.Interfaces;

namespace SmartBudget.Mobile.Services;

public class ApiExpenseService : IExpenseService
{
    private readonly HttpClient _http;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public ApiExpenseService(IHttpClientFactory httpFactory)
    {
        _http = httpFactory.CreateClient("api");
    }

    public async Task<IReadOnlyList<ExpenseCategory>> GetCategoriesAsync()
    {
        try
        {
            var resp = await _http.GetAsync("expenses/categories");
            var body = await resp.Content.ReadAsStringAsync();
            if (resp.IsSuccessStatusCode && !string.IsNullOrWhiteSpace(body))
            {
                var parsed = JsonSerializer.Deserialize<ApiResponse<List<ExpenseCategory>>>(body, JsonOpts);
                if (parsed?.Success == true && parsed.Data is not null)
                    return parsed.Data;
            }
        }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[categories] {ex.Message}"); }
        return new List<ExpenseCategory>();
    }

    public async Task<IReadOnlyList<Expense>> GetExpensesAsync()
    {
        try
        {
            var resp = await _http.GetAsync("expenses");
            var body = await resp.Content.ReadAsStringAsync();
            if (resp.IsSuccessStatusCode && !string.IsNullOrWhiteSpace(body))
            {
                var parsed = JsonSerializer.Deserialize<ApiResponse<List<Expense>>>(body, JsonOpts);
                if (parsed?.Success == true && parsed.Data is not null)
                    return parsed.Data;
            }
        }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[expenses] {ex.Message}"); }
        return new List<Expense>();
    }

    public async Task<ApiResponse<Expense>> AddExpenseAsync(CreateExpenseRequest request)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("expenses", request);
            var body = await resp.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(body))
            {
                var parsed = JsonSerializer.Deserialize<ApiResponse<Expense>>(body, JsonOpts);
                if (parsed is not null) return parsed;
            }
            return new ApiResponse<Expense> { Success = false, Error = $"Server error ({(int)resp.StatusCode})." };
        }
        catch (Exception ex)
        {
            return new ApiResponse<Expense> { Success = false, Error = $"Network error: {ex.Message}" };
        }
    }

    public async Task<decimal> GetTotalAsync()
    {
        var items = await GetExpensesAsync();
        return items.Sum(e => e.Amount);
    }
}