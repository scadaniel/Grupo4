using App_v3.ViewModels;

namespace App_v3.Views;

/// <summary>
/// Base de las pantallas: cada vez que la pantalla aparece le pide a su ViewModel que vuelva a leer los datos.
/// Además limita el contenido a un ancho máximo y lo centra, para que con la ventana ancha no se estire todo.
/// Las pantallas heredan de esta clase y solo reciben su ViewModel; no tienen más código.
/// </summary>
public class PaginaBase : ContentPage
{
    private const double AnchoMaximoContenido = 900;

    /// <summary>"← Volver" de las páginas secundarias que no tienen comando propio (mismo efecto que la flecha nativa).</summary>
    public async void AlVolver(object? sender, EventArgs e) => await Shell.Current.GoToAsync("..");

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is IPantalla pantalla) await pantalla.AlMostrarseAsync();
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        if (width <= 0) return;

        // Con ventana más ancha que el máximo, el sobrante se reparte como margen a los dos lados.
        var lateral = Math.Max(0, (width - AnchoMaximoContenido) / 2);
        if (Math.Abs(Padding.Left - lateral) > 0.5) Padding = new Thickness(lateral, 0);
    }
}