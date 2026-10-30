using App_v3.ViewModels;

namespace App_v3.Views;

public partial class SeguimientoPage : PaginaBase
{
    public SeguimientoPage(SeguimientoViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
