using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartBudget.Mobile.Services.Interfaces;
using SmartBudget.Mobile.Views;

namespace SmartBudget.Mobile.ViewModels;

[QueryProperty(nameof(Email), "email")]
public partial class ResetPasswordViewModel : BaseViewModel
{
    private readonly IAuthService _authService;

    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string code = string.Empty;
    [ObservableProperty] private string newPassword = string.Empty;
    [ObservableProperty] private string confirmPassword = string.Empty;
    [ObservableProperty] private string? errorMessage;

    public ResetPasswordViewModel(IAuthService authService)
    {
        _authService = authService;
        Title = "Reset Password";
    }

    [RelayCommand]
    private async Task ResetAsync()
    {
        if (IsBusy) return;
        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(Code) || Code.Trim().Length != 6)
        { ErrorMessage = "Enter the 6-digit code from your email."; return; }
        if (NewPassword.Length < 8 || !NewPassword.Any(char.IsUpper) || !NewPassword.Any(char.IsDigit))
        { ErrorMessage = "Password needs 8+ chars, an uppercase letter and a number."; return; }
        if (NewPassword != ConfirmPassword)
        { ErrorMessage = "Passwords do not match."; return; }

        try
        {
            IsBusy = true;
            var response = await _authService.ResetPasswordAsync(Email, Code, NewPassword);

            if (response.Success)
            {
                await Shell.Current.DisplayAlert("Done", "Password reset. Please sign in.", "OK");
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
            else
            {
                ErrorMessage = response.Error ?? "Reset failed.";
            }
        }
        finally { IsBusy = false; }
    }
}