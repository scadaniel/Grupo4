using App_v3.ViewModels;

namespace App_v3.Views;

public partial class EntregasPage : PaginaBase
{
    public EntregasPage(EntregasViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
