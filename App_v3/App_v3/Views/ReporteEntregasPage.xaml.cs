using App_v3.ViewModels;

namespace App_v3.Views;

public partial class ReporteEntregasPage : PaginaBase
{
    public ReporteEntregasPage(ReporteEntregasViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
