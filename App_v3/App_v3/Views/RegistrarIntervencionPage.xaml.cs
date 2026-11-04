using App_v3.ViewModels;

namespace App_v3.Views;

public partial class RegistrarIntervencionPage : ContentPage
{
    public RegistrarIntervencionPage(RegistrarIntervencionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}