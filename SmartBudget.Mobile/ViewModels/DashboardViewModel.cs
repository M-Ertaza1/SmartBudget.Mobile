using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartBudget.Mobile.Helpers;
using SmartBudget.Mobile.Models;
using SmartBudget.Mobile.Services.Interfaces;
using SmartBudget.Mobile.Views;

namespace SmartBudget.Mobile.ViewModels;

public partial class DashboardViewModel : BaseViewModel
{
    private readonly ICycleService _cycleService;
    private readonly IAuthService _authService;
    private readonly IExpenseService _expenseService;
    private readonly IIncomeService _incomeService;

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

    public ObservableCollection<Expense> Expenses { get; } = new();

    public DashboardViewModel(ICycleService cycleService, IAuthService authService,
                              IExpenseService expenseService, IIncomeService incomeService)
    {
        _cycleService = cycleService;
        _authService = authService;
        _expenseService = expenseService;
        _incomeService = incomeService;
        Title = "Dashboard";
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;

            // Cycle + totals now come from the API (salaryDay arg is ignored server-side).
            var summary = await _cycleService.GetActiveCycleAsync(0);
            CycleRange = $"{summary.StartDate:dd MMM} – {summary.EndDate:dd MMM yyyy}";

            TotalIncome = summary.TotalIncome;
            TotalExpenses = summary.TotalExpenses;
            Balance = summary.Balance;

            // Expenses list still comes from the mock for now (replaced next step).
            var added = await _expenseService.GetExpensesAsync();
            Expenses.Clear();
            foreach (var e in added) Expenses.Add(e);
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task AddExpenseAsync()
        => await Shell.Current.GoToAsync(nameof(Views.AddExpensePage));

    [RelayCommand]
    private async Task AddIncomeAsync()
        => await Shell.Current.GoToAsync(nameof(Views.AddIncomePage));

    [RelayCommand]
    private async Task LogoutAsync()
    {
        await _authService.LogoutAsync();
        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
    }
}