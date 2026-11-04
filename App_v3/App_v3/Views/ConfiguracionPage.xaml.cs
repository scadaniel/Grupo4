namespace App_v3.Views;

public partial class ConfiguracionPage : ContentPage
{
    public ConfiguracionPage()
    {
        InitializeComponent();
    }

    private void OnBotonMenuFlyoutClicked(object sender, EventArgs e)
    {
        // Forzamos la apertura del menú lateral de forma directa
        App.Current.MainPage = App.Current.MainPage; // Asegura el contexto activo si fuese necesario

        if (Shell.Current != null)
        {
            Shell.Current.FlyoutIsPresented = !Shell.Current.FlyoutIsPresented;
        }
    }
}