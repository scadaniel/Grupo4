
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

        var ventana = new Window(new ContentPage
        {
            Content = new ActivityIndicator
            {
                IsRunning = true,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            }
        });

        _ = InicializarAsync(ventana, datos);

        return ventana;
    }

    private static async Task InicializarAsync(Window ventana, DatosDeEjemplo datos)
    {
        try
        {
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
