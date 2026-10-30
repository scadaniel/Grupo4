using App_v3.ViewModels;

namespace App_v3.Views;

public partial class ReporteAsistenciasPage : PaginaBase
{
    public ReporteAsistenciasPage(ReporteAsistenciasViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
