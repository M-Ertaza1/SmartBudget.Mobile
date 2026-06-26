using SmartBudget.Mobile.Helpers;
using SmartBudget.Mobile.Models;
using SmartBudget.Mobile.Services.Interfaces;

namespace SmartBudget.Mobile.Services;

// TEMPORARY: simulates the backend so the UI is testable before the API exists.
// Swap to the real AuthService (one line in MauiProgram.cs) once the API is running.
public class MockAuthService : IAuthService
{
    public async Task<ApiResponse<AuthResult>> LoginAsync(string email, string password)
    {
        await Task.Delay(1000); // pretend we're calling a server

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return new ApiResponse<AuthResult> { Success = false, Error = "Email and password are required." };

        var result = new AuthResult
        {
            Token = "mock-jwt-token",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(24),
            User = new UserSummary { UserId = Guid.NewGuid(), FullName = "Test User", Plan = "free" }
        };

        try { await SecureStorage.SetAsync(Constants.AuthTokenKey, result.Token); }
        catch { /* SecureStorage can be flaky on some Windows setups; ignore for the mock */ }

        return new ApiResponse<AuthResult> { Success = true, Data = result };
    }

    public async Task<bool> IsLoggedInAsync()
    {
        try { return !string.IsNullOrEmpty(await SecureStorage.GetAsync(Constants.AuthTokenKey)); }
        catch { return false; }
    }

    public Task LogoutAsync()
    {
        try { SecureStorage.Remove(Constants.AuthTokenKey); } catch { }
        return Task.CompletedTask;
    }
}