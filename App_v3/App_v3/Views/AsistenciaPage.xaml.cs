using App_v3.ViewModels;

namespace App_v3.Views;

public partial class AsistenciaPage : PaginaBase
{
    public AsistenciaPage(AsistenciaViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
