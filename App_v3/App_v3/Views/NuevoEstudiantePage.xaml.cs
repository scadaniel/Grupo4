using App_v3.ViewModels;

namespace App_v3.Views;

public partial class NuevoEstudiantePage : ContentPage
{
    public NuevoEstudiantePage(NuevoEstudianteViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
