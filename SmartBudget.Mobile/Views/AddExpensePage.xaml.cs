using SmartBudget.Mobile.ViewModels;

namespace SmartBudget.Mobile.Views;

public partial class AddExpensePage : ContentPage
{
    private readonly AddExpenseViewModel _vm;

    public AddExpensePage(AddExpenseViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _vm = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadCategoriesCommand.ExecuteAsync(null);
    }
}