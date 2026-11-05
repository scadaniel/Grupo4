namespace App_v3.Views;

public partial class BienvenidaPage : ContentPage
{
    public BienvenidaPage()
    {
        InitializeComponent();
    }

    private void OnResponsableClicked(object sender, EventArgs e)
    {
        Application.Current!.Windows[0].Page = new AppShell();
    }

    private async void OnEstudianteClicked(object sender, EventArgs e)
    {
        try
        {
            var portalPage = IPlatformApplication.Current!.Services
                .GetRequiredService<EstudiantePortalPage>();

            Application.Current!.Windows[0].Page =
                new NavigationPage(portalPage);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Error",
                $"No se pudo abrir el portal: {ex.Message}",
                "Aceptar");
        }
    }
}