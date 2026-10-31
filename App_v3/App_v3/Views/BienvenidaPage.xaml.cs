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
        // Vista de estudiante (puedes adaptarla después)
        await DisplayAlert("Acceso", "Redirigiendo al panel de Estudiante...", "OK");
    }
}