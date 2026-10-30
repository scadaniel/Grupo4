using App_v3.ViewModels;

namespace App_v3.Views;

public partial class ComisionesNominaPage : ContentPage
{
    public ComisionesNominaPage(ComisionesNominaViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}