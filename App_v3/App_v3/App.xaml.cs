using App_v3.Dominio;
using Microsoft.Extensions.DependencyInjection;

namespace App_v3;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var servicios = IPlatformApplication.Current!.Services;
        var datos = servicios.GetRequiredService<DatosDeEjemplo>();
        var repositorio = servicios.GetRequiredService<IRepositorio>();

        var ventana = new Window(new ContentPage
        {
            Content = new ActivityIndicator
            {
                IsRunning = true,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            }
        });

        _ = InicializarAsync(ventana, datos, repositorio);

        return ventana;
    }

    private static async Task InicializarAsync(Window ventana, DatosDeEjemplo datos, IRepositorio repositorio)
    {
        try
        {
            // Forzamos la limpieza y recarga de los datos originales limpios para sanear la BD
            await datos.CargarAsync();

            ventana.Page = new Views.BienvenidaPage();
        }
        catch (Exception ex)
        {
            ventana.Page = new ContentPage
            {
                Content = new VerticalStackLayout
                {
                    Padding = 24,
                    Children =
                    {
                        new Label { Text = "Error al cargar los datos" },
                        new Label { Text = ex.Message }
                    }
                }
            };
        }
    }
}