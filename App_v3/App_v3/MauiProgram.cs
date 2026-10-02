using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
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
        builder.Logging.AddDebug();
#endif

        // Inyección de dependencias
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddTransient<GestionPersonasViewModel>();
        builder.Services.AddTransient<GestionPersonasPage>();

        return builder.Build();
    }
}