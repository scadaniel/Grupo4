using App_v3.Dominio;
using App_v3.Plataforma;
using App_v3.ViewModels;
using App_v3.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace App_v3;

public static class MauiProgram
{
    private const string NombreBaseDeDatos = "SeguimientoAcademico.db3";

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
        builder.Logging.AddDebug();
#endif

        RegistrarPersistenciaYPlataforma(builder.Services);
        RegistrarServiciosDeDominio(builder.Services);
        RegistrarViewModels(builder.Services);
        RegistrarPaginas(builder.Services);

        return builder.Build();
    }

    // Lo único que sabe de SQLite y de Shell. El dominio recibe estas piezas por sus interfaces.
    private static void RegistrarPersistenciaYPlataforma(IServiceCollection servicios)
    {
        servicios.AddSingleton<IRepositorio>(_ =>
            new RepositorioSqlite(Path.Combine(FileSystem.AppDataDirectory, NombreBaseDeDatos)));
        servicios.AddSingleton(TimeProvider.System);
        servicios.AddSingleton<IDialogService, ShellDialogService>();
        servicios.AddSingleton<INavegacionService, ShellNavegacionService>();
        servicios.AddSingleton<IEnlaceService, LauncherEnlaceService>();
    }

    private static void RegistrarServiciosDeDominio(IServiceCollection servicios)
    {
        servicios.AddSingleton<MateriaService>();
        servicios.AddSingleton<ComisionService>();
        servicios.AddSingleton<EstudianteService>();
        servicios.AddSingleton<InscripcionService>();
        servicios.AddSingleton<UmbralesService>();
        servicios.AddSingleton<CalendarioService>();
        servicios.AddSingleton<AsistenciaService>();
        servicios.AddSingleton<EntregaService>();
        servicios.AddSingleton<SeguimientoService>();
        servicios.AddSingleton<ContactoService>();
        servicios.AddSingleton<ReporteService>();
        servicios.AddSingleton<DatosDeEjemplo>();
    }

    // El selector de materia y comisión es uno solo para toda la app; el resto se arma de nuevo en cada uso.
    private static void RegistrarViewModels(IServiceCollection servicios)
    {
        servicios.AddSingleton<SelectorComisionViewModel>();
        servicios.AddTransient<FormularioContactoViewModel>();
        servicios.AddTransient<SeguimientoViewModel>();
        servicios.AddTransient<EstudiantesViewModel>();
        servicios.AddTransient<MateriasViewModel>();
        servicios.AddTransient<CalendarioViewModel>();
        servicios.AddTransient<AsistenciaViewModel>();
        servicios.AddTransient<EntregasViewModel>();
        servicios.AddTransient<MenuReportesViewModel>();
        servicios.AddTransient<ReporteAsistenciasViewModel>();
        servicios.AddTransient<ReporteEntregasViewModel>();
        servicios.AddTransient<UmbralesViewModel>();
        servicios.AddTransient<DatosViewModel>();
        servicios.AddTransient<ConfiguracionViewModel>();
    }

    private static void RegistrarPaginas(IServiceCollection servicios)
    {
        servicios.AddTransient<SeguimientoPage>();
        servicios.AddTransient<EstudiantesPage>();
        servicios.AddTransient<MateriasPage>();
        servicios.AddTransient<CalendarioPage>();
        servicios.AddTransient<AsistenciaPage>();
        servicios.AddTransient<EntregasPage>();
        servicios.AddTransient<MenuReportesPage>();
        servicios.AddTransient<ReporteAsistenciasPage>();
        servicios.AddTransient<ReporteEntregasPage>();
        servicios.AddTransient<ConfiguracionPage>();
    }
}
