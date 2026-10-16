namespace App_v3;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // --- REGISTRO OBLIGATORIO DE RUTAS PARA NAVEGACIÓN EN MAUI ---
        Routing.RegisterRoute(nameof(Views.MenuReportesPage), typeof(Views.MenuReportesPage));
        Routing.RegisterRoute(nameof(Views.ReporteAsistenciasPage), typeof(Views.ReporteAsistenciasPage));
        Routing.RegisterRoute(nameof(Views.ReporteEntregasTpPage), typeof(Views.ReporteEntregasTpPage));

        MainPage = new AppShell();
    }
}