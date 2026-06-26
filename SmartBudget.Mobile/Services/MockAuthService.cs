using SmartBudget.Mobile.Helpers;
using SmartBudget.Mobile.Models;
using SmartBudget.Mobile.Services.Interfaces;

namespace SmartBudget.Mobile.Services;

// TEMPORARY: simulates the backend so the UI is testable before the API exists.
// Swap to the real AuthService (one line in MauiProgram.cs) once the API is running.
public class MockAuthService : IAuthService
{
    // DEV ONLY: the real API emails a random code. The mock accepts this fixed one.
    private const string DevCode = "123456";

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

    public async Task<ApiResponse<RegisterResult>> RegisterAsync(RegisterRequest request)
    {
        await Task.Delay(1000); // simulate the API call

        return new ApiResponse<RegisterResult>
        {
            Success = true,
            Data = new RegisterResult
            {
                UserId = Guid.NewGuid(),
                Message = "Verification email sent"
            },
            Message = "Verification email sent"
        };
    }

    public async Task<ApiResponse<bool>> VerifyEmailAsync(string email, string code)
    {
        await Task.Delay(800);

        if (code?.Trim() == DevCode)
            return new ApiResponse<bool> { Success = true, Data = true, Message = "Email verified." };

        return new ApiResponse<bool> { Success = false, Error = "Invalid or expired code." };
    }

    public async Task<ApiResponse<bool>> ResendCodeAsync(string email)
    {
        await Task.Delay(600);
        return new ApiResponse<bool> { Success = true, Data = true, Message = "A new code has been sent." };
    }

    public async Task<ApiResponse<bool>> ForgotPasswordAsync(string email)
    {
        await Task.Delay(800);

        if (string.IsNullOrWhiteSpace(email))
            return new ApiResponse<bool> { Success = false, Error = "Please enter your email." };

        // Real API responds the same whether or not the account exists (privacy).
        return new ApiResponse<bool> { Success = true, Data = true, Message = "If that account exists, a reset code was sent." };
    }

    public async Task<ApiResponse<bool>> ResetPasswordAsync(string email, string code, string newPassword)
    {
        await Task.Delay(800);

        if (code?.Trim() != DevCode)
            return new ApiResponse<bool> { Success = false, Error = "Invalid or expired code." };

        return new ApiResponse<bool> { Success = true, Data = true, Message = "Password reset successful." };
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