using SmartBudget.Mobile.ViewModels;

namespace SmartBudget.Mobile.Views;

public partial class VerifyCodePage : ContentPage
{
    public VerifyCodePage(VerifyCodeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}