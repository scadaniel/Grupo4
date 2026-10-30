using App_v3.ViewModels;

namespace App_v3.Views;

public partial class GestionPersonasMenuPage : ContentPage
{
    public GestionPersonasMenuPage(GestionPersonasMenuViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}