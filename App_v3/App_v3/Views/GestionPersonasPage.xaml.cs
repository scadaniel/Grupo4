using App_v3.ViewModels;

namespace App_v3.Views;

public partial class GestionPersonasPage : ContentPage
{
    public GestionPersonasPage(GestionPersonasViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
