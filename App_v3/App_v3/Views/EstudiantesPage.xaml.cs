using App_v3.ViewModels;

namespace App_v3.Views;

public partial class EstudiantesPage : PaginaBase
{
    public EstudiantesPage(EstudiantesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
