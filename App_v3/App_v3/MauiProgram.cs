using Microsoft.Extensions.Logging;
using App_v3.Core;
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
        builder.Services.AddSingleton<IContactoStore, ContactoStoreSqlite>();
        builder.Services.AddSingleton(sp => new SeguimientoContactos(sp.GetRequiredService<IContactoStore>()));

        // =========================================================
        // MÓDULO: GESTIÓN DE PERSONAS
        // =========================================================
        builder.Services.AddTransient<GestionPersonasViewModel>();
        builder.Services.AddTransient<GestionPersonasPage>();

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
        // Inyección transitoria para instanciar limpia la toma de lista
        // =========================================================
        builder.Services.AddTransient<AttendanceViewModel>();
        builder.Services.AddTransient<AttendancePage>();

        // =========================================================
        // MÓDULO: CALENDARIO Y TRABAJOS PRÁCTICOS (Historia H2)
        // Gestión de fechas límite, clases programadas y cronograma
        // =========================================================
        builder.Services.AddTransient<CalendarViewModel>();
        builder.Services.AddTransient<CalendarPage>();

        // =========================================================
        // MÓDULO: services de configuración de umbrales de riesgo (Historia H4)
        // =========================================================

        builder.Services.AddSingleton<ConfiguracionService>();
        builder.Services.AddTransient<ConfiguracionViewModel>();
        builder.Services.AddTransient<ConfiguracionPage>();

        // =========================================================
        // MÓDULO: registro de entregas de trabajos prácticos
        // =========================================================

        builder.Services.AddTransient<RegistroEntregasViewModel>();

        // =========================================================
        // MÓDULO: toma de asistencia de alumnos (Historia H3)
        // =========================================================

        builder.Services.AddTransient<TomaAsistenciaViewModel>();
        builder.Services.AddTransient<TomaAsistenciaPage>();

        // =========================================================
        // MÓDULO: Entrega de trabajos prácticos 
        // =========================================================

        builder.Services.AddTransient<RegistroEntregasViewModel>();
        builder.Services.AddTransient<RegistroEntregasPage>();

        // =========================================================
        // reporte:  Reportes de Asistencia
        // =========================================================
        builder.Services.AddTransient<ReporteAsistenciasViewModel>();
        builder.Services.AddTransient<ReporteAsistenciasPage>();

        // =========================================================
        // reporte: Entrega de trabajos prácticos 
        // =========================================================

        builder.Services.AddTransient<ReporteEntregasTpViewModel>();
        builder.Services.AddTransient<ReporteEntregasTpPage>();


        // =========================================================
        // menu de reportes
        // =========================================================

        builder.Services.AddTransient<MenuReportesPage>();

        return builder.Build();
    }
}