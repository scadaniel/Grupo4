using App_v3.ViewModels;

namespace App_v3.Views;

/// <summary>
/// Base de las pantallas: cada vez que la pantalla aparece le pide a su ViewModel que vuelva a leer los datos.
/// Las pantallas heredan de esta clase y solo reciben su ViewModel; no tienen más código.
/// </summary>
public class PaginaBase : ContentPage
{
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is IPantalla pantalla) await pantalla.AlMostrarseAsync();
    }
}
