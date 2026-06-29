using SmartBudget.Mobile.ViewModels;

namespace SmartBudget.Mobile.Views;

public partial class AddIncomePage : ContentPage
{
    public AddIncomePage(AddIncomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}