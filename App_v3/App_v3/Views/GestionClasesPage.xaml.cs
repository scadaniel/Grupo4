using App_v3.ViewModels;

namespace App_v3.Views;

public partial class GestionClasesPage : ContentPage
{
    public GestionClasesPage(GestionClasesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}