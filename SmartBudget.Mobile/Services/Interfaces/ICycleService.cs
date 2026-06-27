using SmartBudget.Mobile.Models;

namespace SmartBudget.Mobile.Services.Interfaces;

public interface ICycleService
{
    // Pure date logic — the heart of the app. Runs on-device, no server needed.
    (DateOnly Start, DateOnly End) CalculateCycleDates(int salaryDay, DateOnly reference);

    // Active cycle with totals (totals are mocked until the API exists).
    Task<CycleSummary> GetActiveCycleAsync(int salaryDay);
}