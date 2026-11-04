using Microsoft.Extensions.Logging;
using App_v3.Services;
using App_v3.ViewModels;
using App_v3.Views;

namespace App_v3;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        // Configuración de registros/logs para entorno de depuración
        builder.Logging.AddDebug();
#endif

        // =========================================================
        // CAPA DE SERVICIOS / PERSISTENCIA (SINGLETON)
        // Se mantiene una única instancia compartida en toda la App
        // =========================================================
        builder.Services.AddSingleton<DatabaseService>();

        // =========================================================
        // MÓDULO: PANTALLA DE BIENVENIDA (INICIO)
        // =========================================================
        builder.Services.AddTransient<WelcomePage>();

        // =========================================================
        // MÓDULO: GESTIÓN DE CLASES Y CÁTEDRAS
        // =========================================================
        builder.Services.AddTransient<GestionClasesViewModel>();
        builder.Services.AddTransient<GestionClasesPage>();

        // =========================================================
        // MÓDULO: ESTUDIANTES / RIESGO ACADÉMICO
        // =========================================================
        builder.Services.AddTransient<EstudianteListViewModel>();
        builder.Services.AddTransient<EstudianteListPage>();

        // =========================================================
        // MÓDULO: ASISTENCIA DE ALUMNOS (Historia H3)
        // =========================================================
        builder.Services.AddTransient<AttendanceViewModel>();
        builder.Services.AddTransient<AttendancePage>();

        // =========================================================
        // MÓDULO: CALENDARIO Y TRABAJOS PRÁCTICOS (Historia H2)
        // =========================================================
        builder.Services.AddTransient<CalendarViewModel>();
        builder.Services.AddTransient<CalendarPage>();

        // =========================================================
        // MÓDULO: CONFIGURACIÓN DE UMBRALES DE RIESGO (Historia H4)
        // =========================================================
        builder.Services.AddSingleton<ConfiguracionService>();
        builder.Services.AddTransient<ConfiguracionViewModel>();
        builder.Services.AddTransient<ConfiguracionPage>();

        // =========================================================
        // MÓDULO: TOMA DE ASISTENCIA DE ALUMNOS (Historia H3)
        // =========================================================
        builder.Services.AddTransient<TomaAsistenciaViewModel>();
        builder.Services.AddTransient<TomaAsistenciaPage>();

        // =========================================================
        // MÓDULO: ENTREGA DE TRABAJOS PRÁCTICOS 
        // =========================================================
        builder.Services.AddTransient<RegistroEntregasViewModel>();
        builder.Services.AddTransient<RegistroEntregasPage>();

        // =========================================================
        // REPORTES: Reportes de Asistencia
        // =========================================================
        builder.Services.AddTransient<ReporteAsistenciasViewModel>();
        builder.Services.AddTransient<ReporteAsistenciasPage>();

        // =========================================================
        // REPORTES: Entrega de trabajos prácticos 
        // =========================================================
        builder.Services.AddTransient<ReporteEntregasTpViewModel>();
        builder.Services.AddTransient<ReporteEntregasTpPage>();

        // =========================================================
        // MENÚ DE REPORTES
        // =========================================================
        builder.Services.AddTransient<MenuReportesPage>();

        // =========================================================
        // MENÚ DE ESTUDIANTES / GESTIÓN DE PERSONAS
        // =========================================================
        builder.Services.AddTransient<GestionPersonasMenuPage>();
        builder.Services.AddTransient<GestionPersonasMenuViewModel>();

        builder.Services.AddTransient<ListadoEstudiantesPage>();
        builder.Services.AddTransient<ListadoEstudiantesViewModel>();

        builder.Services.AddTransient<ComisionesNominaPage>();
        builder.Services.AddTransient<ComisionesNominaViewModel>();

        builder.Services.AddTransient<NuevoEstudiantePage>();
        builder.Services.AddTransient<NuevoEstudianteViewModel>();

        return builder.Build();
    }
}