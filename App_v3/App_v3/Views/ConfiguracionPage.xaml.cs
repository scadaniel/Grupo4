using App_v3.ViewModels;
using App_v3.Services;

namespace App_v3.Views;

public partial class ConfiguracionPage : ContentPage
{
    // Opción A: Si usas Inyección de Dependencias (Registrado en MauiProgram.cs)
    public ConfiguracionPage(ConfiguracionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    /* 
    // Opción B: Si prefieres instanciarlo directamente sin tocar MauiProgram.cs, 
    // usa este constructor en su lugar y elimina el de arriba:
    public ConfiguracionPage()
    {
        InitializeComponent();
        BindingContext = new ConfiguracionViewModel(new DatabaseService());
    }
    */

    private void OnBotonMenuFlyoutClicked(object sender, EventArgs e)
    {
        if (Shell.Current != null)
        {
            Shell.Current.FlyoutIsPresented = !Shell.Current.FlyoutIsPresented;
        }
    }
}