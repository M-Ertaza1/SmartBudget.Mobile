namespace SmartBudget.API.Helpers;

public static class CycleCalculator
{
    // salaryDay=5, reference=2026-06-20  ->  (2026-06-05, 2026-07-04)
    public static (DateOnly Start, DateOnly End) GetCycleDates(int salaryDay, DateOnly reference)
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

    private static DateOnly SafeDate(int year, int month, int day)
    {
        int lastDay = DateTime.DaysInMonth(year, month);
        return new DateOnly(year, month, Math.Min(day, lastDay));
    }
}