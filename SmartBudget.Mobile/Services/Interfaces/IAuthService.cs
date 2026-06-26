using SmartBudget.Mobile.Models;

namespace SmartBudget.Mobile.Services.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<AuthResult>> LoginAsync(string email, string password);
    Task<bool> IsLoggedInAsync();
    Task LogoutAsync();
}