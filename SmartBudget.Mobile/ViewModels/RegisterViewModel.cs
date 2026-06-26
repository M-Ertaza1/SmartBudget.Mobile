using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartBudget.Mobile.Models;
using SmartBudget.Mobile.Services.Interfaces;
using SmartBudget.Mobile.Views;

namespace SmartBudget.Mobile.ViewModels;

public partial class RegisterViewModel : BaseViewModel
{
    private readonly IAuthService _authService;

    [ObservableProperty] private string fullName = string.Empty;
    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string mobile = string.Empty;
    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private string confirmPassword = string.Empty;
    [ObservableProperty] private string? errorMessage;

    public RegisterViewModel(IAuthService authService)
    {
        _authService = authService;
        Title = "Register";
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        if (IsBusy) return;
        ErrorMessage = null;

        var problem = Validate();
        if (problem is not null) { ErrorMessage = problem; return; }

        try
        {
            IsBusy = true;

            var request = new RegisterRequest
            {
                FullName = FullName.Trim(),
                Email = Email.Trim(),
                Mobile = Mobile.Trim(),
                Password = Password,
                ConfirmPassword = ConfirmPassword
            };

            var response = await _authService.RegisterAsync(request);

            if (response.Success)
            {
                // Straight to the verify-code screen, passing the email along.
                await Shell.Current.GoToAsync(
                    $"{nameof(VerifyCodePage)}?email={Uri.EscapeDataString(request.Email)}");
            }
            else
            {
                ErrorMessage = response.Error ?? "Registration failed. Please try again.";
            }
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
    private async Task GoToLoginAsync() => await Shell.Current.GoToAsync("..");

    private string? Validate()
    {
        if (string.IsNullOrWhiteSpace(FullName))
            return "Please enter your full name.";

        if (string.IsNullOrWhiteSpace(Email) || !Email.Contains('@') || !Email.Contains('.'))
            return "Please enter a valid email address.";

        var digits = new string(Mobile.Where(char.IsDigit).ToArray());
        if (digits.Length < 10)
            return "Please enter a valid mobile number.";

        if (Password.Length < 8)
            return "Password must be at least 8 characters.";
        if (!Password.Any(char.IsUpper))
            return "Password must contain at least one uppercase letter.";
        if (!Password.Any(char.IsDigit))
            return "Password must contain at least one number.";
        if (Password != ConfirmPassword)
            return "Passwords do not match.";

        return null;
    }
}