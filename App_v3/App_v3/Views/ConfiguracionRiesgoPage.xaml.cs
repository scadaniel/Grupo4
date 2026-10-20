using App_v3.ViewModels;

namespace App_v3.Views;

public partial class ConfiguracionRiesgoPage : ContentPage
{
    public ConfiguracionRiesgoPage(ConfiguracionRiesgoViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
