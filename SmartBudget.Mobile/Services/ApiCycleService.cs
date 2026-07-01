using System.Text.Json;
using SmartBudget.Mobile.Models;
using SmartBudget.Mobile.Services.Interfaces;

namespace SmartBudget.Mobile.Services;

public class ApiCycleService : ICycleService
{
    private readonly HttpClient _http;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public ApiCycleService(IHttpClientFactory httpFactory)
    {
        _http = httpFactory.CreateClient("api");
    }

    // Kept for interface compatibility; the API now owns the real date math.
    public (DateOnly Start, DateOnly End) CalculateCycleDates(int salaryDay, DateOnly reference)
    {
        DateOnly start = reference.Day >= salaryDay
            ? new DateOnly(reference.Year, reference.Month, Math.Min(salaryDay, DateTime.DaysInMonth(reference.Year, reference.Month)))
            : SafePrev(reference, salaryDay);
        return (start, start.AddMonths(1).AddDays(-1));
    }

    private static DateOnly SafePrev(DateOnly r, int day)
    {
        var p = r.AddMonths(-1);
        return new DateOnly(p.Year, p.Month, Math.Min(day, DateTime.DaysInMonth(p.Year, p.Month)));
    }

    public async Task<CycleSummary> GetActiveCycleAsync(int salaryDay)
    {
        try
        {
            var resp = await _http.GetAsync("cycles/active");
            var body = await resp.Content.ReadAsStringAsync();

            System.Diagnostics.Debug.WriteLine($"[cycles/active] status={(int)resp.StatusCode} body='{body}'");

            if (resp.IsSuccessStatusCode && !string.IsNullOrWhiteSpace(body))
            {
                var parsed = JsonSerializer.Deserialize<ApiResponse<CycleSummary>>(body, JsonOpts);
                if (parsed?.Success == true && parsed.Data is not null)
                    return parsed.Data;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[cycles/active] error: {ex.Message}");
        }

        // Fallback so the dashboard doesn't crash if the call fails.
        return new CycleSummary
        {
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            EndDate = DateOnly.FromDateTime(DateTime.Today),
            TotalIncome = 0,
            TotalExpenses = 0
        };
    }
}