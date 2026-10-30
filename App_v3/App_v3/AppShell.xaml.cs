using App_v3.ViewModels;
using App_v3.Views;
using Microsoft.Extensions.DependencyInjection;

namespace App_v3;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // La barra global usa el mismo selector (singleton) que todas las pantallas.
        Barra.BindingContext = IPlatformApplication.Current!.Services.GetRequiredService<SelectorComisionViewModel>();

        // Páginas a las que se llega desde otra página o desde la barra (no son pestañas).
        Routing.RegisterRoute(Rutas.Reportes, typeof(MenuReportesPage));
        Routing.RegisterRoute(Rutas.Configuracion, typeof(ConfiguracionPage));
        Routing.RegisterRoute(Rutas.ReporteAsistencias, typeof(ReporteAsistenciasPage));
        Routing.RegisterRoute(Rutas.ReporteEntregas, typeof(ReporteEntregasPage));
    }
}