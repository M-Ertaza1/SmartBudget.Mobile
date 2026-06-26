using SmartBudget.Mobile.ViewModels;

namespace SmartBudget.Mobile.Views;

public partial class ResetPasswordPage : ContentPage
{
    public ResetPasswordPage(ResetPasswordViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}