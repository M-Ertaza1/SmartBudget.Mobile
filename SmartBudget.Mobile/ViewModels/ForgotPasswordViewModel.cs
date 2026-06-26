using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartBudget.Mobile.Services.Interfaces;
using SmartBudget.Mobile.Views;

namespace SmartBudget.Mobile.ViewModels;

public partial class ForgotPasswordViewModel : BaseViewModel
{
    private readonly IAuthService _authService;

    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string? errorMessage;

    public ForgotPasswordViewModel(IAuthService authService)
    {
        _authService = authService;
        Title = "Forgot Password";
    }

    [RelayCommand]
    private async Task SendCodeAsync()
    {
        if (IsBusy) return;
        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(Email) || !Email.Contains('@'))
        {
            ErrorMessage = "Please enter a valid email address.";
            return;
        }

        try
        {
            IsBusy = true;
            var response = await _authService.ForgotPasswordAsync(Email.Trim());

            if (response.Success)
            {
                await Shell.Current.GoToAsync(
                    $"{nameof(ResetPasswordPage)}?email={Uri.EscapeDataString(Email.Trim())}");
            }
            else
            {
                ErrorMessage = response.Error ?? "Could not send reset code.";
            }
        }
        finally { IsBusy = false; }
    }
}