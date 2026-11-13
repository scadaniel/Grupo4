using App_v3.ViewModels;

namespace App_v3.Views;

public partial class ReporteRiesgoPage : PaginaBase
{
    public ReporteRiesgoPage(ReporteRiesgoViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
