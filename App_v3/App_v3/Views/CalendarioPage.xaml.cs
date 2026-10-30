using App_v3.ViewModels;

namespace App_v3.Views;

public partial class CalendarioPage : PaginaBase
{
    public CalendarioPage(CalendarioViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
