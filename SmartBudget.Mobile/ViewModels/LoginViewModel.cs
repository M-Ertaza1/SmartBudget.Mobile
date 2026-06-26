using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartBudget.Mobile.Services.Interfaces;
using SmartBudget.Mobile.Views;

namespace SmartBudget.Mobile.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IAuthService _authService;

    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private string? errorMessage;

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
        Title = "Login";
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (IsBusy) return;
        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Please enter your email and password.";
            return;
        }

        try
        {
            IsBusy = true;
            var response = await _authService.LoginAsync(Email, Password);

            if (response.Success)
                await Shell.Current.GoToAsync($"//{nameof(DashboardPage)}");
            else
                ErrorMessage = response.Error ?? "Login failed. Please try again.";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Something went wrong: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task GoToForgotPasswordAsync()
        => await Shell.Current.GoToAsync(nameof(Views.ForgotPasswordPage));

    [RelayCommand]
    private async Task GoToRegisterAsync()
    {
        await Shell.Current.GoToAsync(nameof(Views.RegisterPage));
    }
}