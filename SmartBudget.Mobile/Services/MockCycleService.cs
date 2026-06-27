using SmartBudget.Mobile.Models;
using SmartBudget.Mobile.Services.Interfaces;

namespace SmartBudget.Mobile.Services;

public class MockCycleService : ICycleService
{
    // Matches Phase 3 fn_get_cycle_dates and Phase 4 CycleService.CalculateCycleDates.
    // Example: salaryDay=5, today=June 20  ->  June 5 to July 4
    public (DateOnly Start, DateOnly End) CalculateCycleDates(int salaryDay, DateOnly reference)
    {
        DateOnly start;

        if (reference.Day >= salaryDay)
            start = SafeDate(reference.Year, reference.Month, salaryDay);
        else
        {
            var prev = reference.AddMonths(-1);
            start = SafeDate(prev.Year, prev.Month, salaryDay);
        }

        var end = start.AddMonths(1).AddDays(-1);
        return (start, end);
    }

    public async Task<CycleSummary> GetActiveCycleAsync(int salaryDay)
    {
        await Task.Delay(500); // simulate fetching totals from the server

        var (start, end) = CalculateCycleDates(salaryDay, DateOnly.FromDateTime(DateTime.Today));

        // Mocked totals — replaced by real API data later.
        return new CycleSummary
        {
            StartDate = start,
            EndDate = end,
            TotalIncome = 85000m,
            TotalExpenses = 41250m
        };
    }

    // Guards against invalid dates like "Feb 31" by clamping to the month's last day.
    private static DateOnly SafeDate(int year, int month, int day)
    {
        int lastDay = DateTime.DaysInMonth(year, month);
        return new DateOnly(year, month, Math.Min(day, lastDay));
    }
}