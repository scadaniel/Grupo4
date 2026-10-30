using App_v3.ViewModels;

namespace App_v3.Views;

public partial class ListadoEstudiantesPage : ContentPage
{
    public ListadoEstudiantesPage(ListadoEstudiantesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
