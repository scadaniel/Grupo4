using App_v3.ViewModels;


namespace App_v3.Views;

public partial class ConfiguracionPage : ContentPage
{
    public ConfiguracionPage(ConfiguracionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}