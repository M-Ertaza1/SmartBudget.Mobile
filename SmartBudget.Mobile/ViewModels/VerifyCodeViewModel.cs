using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartBudget.Mobile.Services.Interfaces;
using SmartBudget.Mobile.Views;

namespace SmartBudget.Mobile.ViewModels;

[QueryProperty(nameof(Email), "email")]
public partial class VerifyCodeViewModel : BaseViewModel
{
    private readonly IAuthService _authService;

    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string code = string.Empty;
    [ObservableProperty] private string? errorMessage;

    public VerifyCodeViewModel(IAuthService authService)
    {
        _authService = authService;
        Title = "Verify Email";
    }

    [RelayCommand]
    private async Task VerifyAsync()
    {
        if (IsBusy) return;
        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(Code) || Code.Trim().Length != 6)
        {
            ErrorMessage = "Enter the 6-digit code sent to your email.";
            return;
        }

        try
        {
            IsBusy = true;
            var response = await _authService.VerifyEmailAsync(Email, Code);

            if (response.Success)
            {
                await Shell.Current.DisplayAlert("Verified", "Your email is verified. Please sign in.", "OK");
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
            else
            {
                ErrorMessage = response.Error ?? "Verification failed.";
            }
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task ResendAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            var response = await _authService.ResendCodeAsync(Email);
            await Shell.Current.DisplayAlert("Code sent", response.Message ?? "A new code was sent.", "OK");
        }
        finally { IsBusy = false; }
    }
}