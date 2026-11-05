namespace App_v3.Views;

public partial class BienvenidaPage : ContentPage
{
    public BienvenidaPage()
    {
        InitializeComponent();
    }

    private void OnResponsableClicked(object sender, EventArgs e)
    {
        // Al elegir Responsable, cargamos el AppShell completo con las pestañas
        Application.Current.MainPage = new AppShell();
    }

    private async void OnEstudianteClicked(object sender, EventArgs e)
    {
        try
        {
            // Resolvemos la página del portal del estudiante por inyección de dependencias
            var portalPage = IPlatformApplication.Current!.Services.GetRequiredService<EstudiantePortalPage>();

            // La envolvemos en una NavigationPage para que tenga barra superior y soporte retroceso de forma segura
            Application.Current!.MainPage = new NavigationPage(portalPage);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo abrir el portal: {ex.Message}", "Aceptar");
        }
    }
}