using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartBudget.Mobile.Helpers;
using SmartBudget.Mobile.Services.Interfaces;
using SmartBudget.Mobile.Views;

namespace SmartBudget.Mobile.ViewModels;

public partial class DashboardViewModel : BaseViewModel
{
    private readonly ICycleService _cycleService;
    private readonly IAuthService _authService;

    [ObservableProperty] private bool hasSalaryDay;
    [ObservableProperty] private int selectedSalaryDay = 1;
    [ObservableProperty] private string cycleRange = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IncomeText))]
    private decimal totalIncome;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ExpensesText))]
    private decimal totalExpenses;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(BalanceText))]
    private decimal balance;

    public string IncomeText => $"{Constants.CurrencySymbol}{TotalIncome:N0}";
    public string ExpensesText => $"{Constants.CurrencySymbol}{TotalExpenses:N0}";
    public string BalanceText => $"{Constants.CurrencySymbol}{Balance:N0}";

    public ObservableCollection<int> Days { get; } = new(Enumerable.Range(1, 31));

    public DashboardViewModel(ICycleService cycleService, IAuthService authService)
    {
        _cycleService = cycleService;
        _authService = authService;
        Title = "Dashboard";
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;

            int salaryDay = Preferences.Default.Get(Constants.SalaryDayKey, 0);
            HasSalaryDay = salaryDay is >= 1 and <= 31;

            if (!HasSalaryDay)
                return; // show the setup card instead

            var summary = await _cycleService.GetActiveCycleAsync(salaryDay);
            CycleRange = $"{summary.StartDate:dd MMM} – {summary.EndDate:dd MMM yyyy}";
            TotalIncome = summary.TotalIncome;
            TotalExpenses = summary.TotalExpenses;
            Balance = summary.Balance;
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task SaveSalaryDayAsync()
    {
        Preferences.Default.Set(Constants.SalaryDayKey, SelectedSalaryDay);
        HasSalaryDay = true;
        await LoadAsync();
    }

    [RelayCommand]
    private async Task ComingSoonAsync(string feature)
        => await Shell.Current.DisplayAlert(feature, "This screen is coming next.", "OK");

    [RelayCommand]
    private async Task LogoutAsync()
    {
        await _authService.LogoutAsync();
        Preferences.Default.Remove(Constants.SalaryDayKey); // reset so you can re-test the setup card
        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
    }
}