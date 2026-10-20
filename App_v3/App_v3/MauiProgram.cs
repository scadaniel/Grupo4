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
        // =========================================================
        builder.Services.AddSingleton<IDatabaseService>(_ =>
            new DatabaseService(Path.Combine(FileSystem.AppDataDirectory, "Asistencias.db3")));
        builder.Services.AddSingleton<IDialogService, ShellDialogService>();

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
        // MÓDULO: UMBRALES DE RIESGO (H7)
        // =========================================================
        builder.Services.AddTransient<ConfiguracionRiesgoViewModel>();
        builder.Services.AddTransient<ConfiguracionRiesgoPage>();

        // =========================================================
        // MÓDULO: ASISTENCIA DE ALUMNOS (H3)
        // =========================================================
        builder.Services.AddTransient<AttendanceViewModel>();
        builder.Services.AddTransient<AttendancePage>();
        builder.Services.AddTransient<TomaAsistenciaViewModel>();
        builder.Services.AddTransient<TomaAsistenciaPage>();

        // =========================================================
        // MÓDULO: CALENDARIO Y TRABAJOS PRÁCTICOS (H2)
        // =========================================================
        builder.Services.AddTransient<CalendarViewModel>();
        builder.Services.AddTransient<CalendarPage>();

        // =========================================================
        // MÓDULO: REGISTRO DE ENTREGAS DE TRABAJOS PRÁCTICOS
        // =========================================================
        builder.Services.AddTransient<RegistroEntregasViewModel>();
        builder.Services.AddTransient<RegistroEntregasPage>();

        // =========================================================
        // MÓDULO: REPORTES
        // =========================================================
        builder.Services.AddTransient<MenuReportesPage>();
        builder.Services.AddTransient<ReporteAsistenciasViewModel>();
        builder.Services.AddTransient<ReporteAsistenciasPage>();
        builder.Services.AddTransient<ReporteEntregasTpViewModel>();
        builder.Services.AddTransient<ReporteEntregasTpPage>();

        return builder.Build();
    }
}