using App_v3.ViewModels;

namespace App_v3.Views;

public partial class MenuReportesPage : PaginaBase
{
    public MenuReportesPage(MenuReportesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
