using App_v3.ViewModels;

namespace App_v3.Views;

public partial class ConfiguracionPage : PaginaBase
{
    public ConfiguracionPage(ConfiguracionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
