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
        }
    }
}