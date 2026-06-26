using SmartBudget.Mobile.Models;

namespace SmartBudget.Mobile.Services.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<AuthResult>> LoginAsync(string email, string password);
    Task<ApiResponse<RegisterResult>> RegisterAsync(RegisterRequest request);

    Task<ApiResponse<bool>> VerifyEmailAsync(string email, string code);
    Task<ApiResponse<bool>> ResendCodeAsync(string email);
    Task<ApiResponse<bool>> ForgotPasswordAsync(string email);
    Task<ApiResponse<bool>> ResetPasswordAsync(string email, string code, string newPassword);

    Task<bool> IsLoggedInAsync();
    Task LogoutAsync();
}