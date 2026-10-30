using App_v3.ViewModels;
using App_v3.Views;

namespace App_v3;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Páginas a las que se llega desde otra página (no son pestañas).
        Routing.RegisterRoute(Rutas.ReporteAsistencias, typeof(ReporteAsistenciasPage));
        Routing.RegisterRoute(Rutas.ReporteEntregas, typeof(ReporteEntregasPage));
    }
}
