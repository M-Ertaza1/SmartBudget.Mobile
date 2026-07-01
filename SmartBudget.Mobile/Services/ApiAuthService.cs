using System.Net.Http.Json;
using System.Net.Http;
using System.Text.Json;
using SmartBudget.Mobile.Helpers;
using SmartBudget.Mobile.Models;
using SmartBudget.Mobile.Services.Interfaces;

namespace SmartBudget.Mobile.Services;

public class ApiAuthService : IAuthService
{
    private readonly HttpClient _http;
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ApiAuthService(IHttpClientFactory httpFactory)
    {
        _http = httpFactory.CreateClient("api");
    }

    public async Task<ApiResponse<AuthResult>> LoginAsync(string email, string password)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("auth/login",
                new { email, password });

            var result = await ReadAsync<AuthResult>(resp);

            if (result.Success && result.Data is not null)
            {
                try { await SecureStorage.SetAsync(Constants.AuthTokenKey, result.Data.Token); }
                catch { /* ignore SecureStorage quirks on Windows */ }
            }
            return result;
        }
        catch (Exception ex)
        {
            return new ApiResponse<AuthResult> { Success = false, Error = $"Network error: {ex.Message}" };
        }
    }

    public async Task<ApiResponse<RegisterResult>> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("auth/register", request);
            return await ReadAsync<RegisterResult>(resp);
        }
        catch (Exception ex)
        {
            return new ApiResponse<RegisterResult> { Success = false, Error = $"Network error: {ex.Message}" };
        }
    }

    public async Task<ApiResponse<bool>> VerifyEmailAsync(string email, string code)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("auth/verify-email", new { email, code });
            return await ReadAsync<bool>(resp);
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool> { Success = false, Error = $"Network error: {ex.Message}" };
        }
    }

    public async Task<ApiResponse<bool>> ResendCodeAsync(string email)
    {
        // Endpoint not built yet on the API — return a friendly placeholder for now.
        await Task.Delay(100);
        return new ApiResponse<bool> { Success = true, Data = true, Message = "Please use the code from registration." };
    }

    public async Task<ApiResponse<bool>> ForgotPasswordAsync(string email)
    {
        await Task.Delay(100);
        return new ApiResponse<bool> { Success = false, Error = "Password reset isn't available yet." };
    }

    public async Task<ApiResponse<bool>> ResetPasswordAsync(string email, string code, string newPassword)
    {
        await Task.Delay(100);
        return new ApiResponse<bool> { Success = false, Error = "Password reset isn't available yet." };
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

    // Reads the API's { success, data, error } envelope regardless of HTTP status.
    private static async Task<ApiResponse<T>> ReadAsync<T>(HttpResponseMessage resp)
    {
        var body = await resp.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(body))
            return new ApiResponse<T> { Success = false, Error = $"Empty response ({(int)resp.StatusCode})." };

        try
        {
            var parsed = JsonSerializer.Deserialize<ApiResponse<T>>(body, JsonOpts);
            return parsed ?? new ApiResponse<T> { Success = false, Error = "Could not read server response." };
        }
        catch
        {
            return new ApiResponse<T> { Success = false, Error = "Unexpected server response." };
        }
    }
}