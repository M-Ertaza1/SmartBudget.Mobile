
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartBudget.Mobile.Models;
using SmartBudget.Mobile.Services.Interfaces;


namespace SmartBudget.Mobile.ViewModels;

public partial class AddIncomeViewModel : BaseViewModel
{
    private readonly IIncomeService _incomeService;

    [ObservableProperty] private string amount = string.Empty;
    [ObservableProperty] private string source = string.Empty;
    [ObservableProperty] private DateTime incomeDate = DateTime.Today;
    [ObservableProperty] private string notes = string.Empty;
    [ObservableProperty] private string? errorMessage;

    public DateTime MaxDate => DateTime.Today;

    public AddIncomeViewModel(IIncomeService incomeService)
    {
        _incomeService = incomeService;
        Title = "Add Income";
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (IsBusy) return;
        ErrorMessage = null;

        if (!decimal.TryParse(Amount, out var value) || value <= 0)
        {
            ErrorMessage = "Enter a valid amount greater than zero.";
            return;
        }
        if (string.IsNullOrWhiteSpace(Source))
        {
            ErrorMessage = "Please enter an income source (e.g. Salary, Freelance).";
            return;
        }

        try
        {
            IsBusy = true;
            var response = await _incomeService.AddIncomeAsync(new CreateIncomeRequest
            {
                Amount = value,
                Source = Source.Trim(),
                IncomeDate = DateOnly.FromDateTime(IncomeDate),
                Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes.Trim()
            });

            if (response.Success)
                await Shell.Current.GoToAsync(".."); // back to dashboard; it refreshes on appear
            else
                ErrorMessage = response.Error ?? "Could not save the income.";
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task CancelAsync() => await Shell.Current.GoToAsync("..");
}