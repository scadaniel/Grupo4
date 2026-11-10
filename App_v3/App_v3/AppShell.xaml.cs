using App_v3.Views;

namespace App_v3
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // REGISTRAR LA RUTA AQUÍ:
            Routing.RegisterRoute(nameof(EstudianteListPage), typeof(EstudianteListPage));
            Routing.RegisterRoute(nameof(ListadoEstudiantesPage), typeof(ListadoEstudiantesPage));
            Routing.RegisterRoute(nameof(ComisionesNominaPage), typeof(ComisionesNominaPage));
            Routing.RegisterRoute(nameof(NuevoEstudiantePage), typeof(NuevoEstudiantePage));
            // Reporte de alumnos contactados y fecha de contacto
            Routing.RegisterRoute(nameof(ReporteContactosPage), typeof(ReporteContactosPage));
            // Reporte de alumnos en riesgo por comisión
            Routing.RegisterRoute(nameof(ReporteRiesgoComisionPage), typeof(ReporteRiesgoComisionPage));
        }
    }
}