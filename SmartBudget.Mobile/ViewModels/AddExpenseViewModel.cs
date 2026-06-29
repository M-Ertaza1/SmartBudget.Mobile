using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartBudget.Mobile.Models;
using SmartBudget.Mobile.Services.Interfaces;
using System.Collections.ObjectModel;

namespace SmartBudget.Mobile.ViewModels;

public partial class AddExpenseViewModel : BaseViewModel
{
    private readonly IExpenseService _expenseService;

    [ObservableProperty] private string amount = string.Empty;
    [ObservableProperty] private ExpenseCategory? selectedCategory;
    [ObservableProperty] private DateTime expenseDate = DateTime.Today;
    [ObservableProperty] private string description = string.Empty;
    [ObservableProperty] private string? errorMessage;

    public ObservableCollection<ExpenseCategory> Categories { get; } = new();

    // Prevents picking a future date in the UI.
    public DateTime MaxDate => DateTime.Today;

    public AddExpenseViewModel(IExpenseService expenseService)
    {
        _expenseService = expenseService;
        Title = "Add Expense";
    }

    [RelayCommand]
    private async Task LoadCategoriesAsync()
    {
        if (Categories.Count > 0) return;
        var cats = await _expenseService.GetCategoriesAsync();
        foreach (var c in cats) Categories.Add(c);
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
        if (SelectedCategory is null)
        {
            ErrorMessage = "Please choose a category.";
            return;
        }
        if (DateOnly.FromDateTime(ExpenseDate) > DateOnly.FromDateTime(DateTime.Today))
        {
            ErrorMessage = "Expense date can't be in the future.";
            return;
        }

        try
        {
            IsBusy = true;
            var response = await _expenseService.AddExpenseAsync(new CreateExpenseRequest
            {
                Amount = value,
                CategoryId = SelectedCategory.CategoryId,
                ExpenseDate = DateOnly.FromDateTime(ExpenseDate),
                Description = string.IsNullOrWhiteSpace(Description) ? null : Description.Trim()
            });

            if (response.Success)
                await Shell.Current.GoToAsync(".."); // back to dashboard; it refreshes on appear
            else
                ErrorMessage = response.Error ?? "Could not save the expense.";
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task CancelAsync() => await Shell.Current.GoToAsync("..");
}