namespace App_v3.ViewModels;

/// <summary>
/// Base de las pantallas que trabajan sobre la comisión elegida arriba.
/// Cuando se elige otra comisión, o cuando la pantalla se muestra, se vuelve a leer lo que corresponde.
/// </summary>
public abstract class PaginaViewModel : BaseViewModel, IPantalla
{
    protected PaginaViewModel(SelectorComisionViewModel selector, IDialogService dialogos) : base(dialogos)
    {
        Selector = selector;
        Selector.ComisionCambiada += (_, _) => _ = EjecutarAsync(CargarAsync);
    }

    public SelectorComisionViewModel Selector { get; }

    /// <summary>La llama la página cada vez que aparece: los datos pueden haber cambiado en otra pestaña.</summary>
    public Task AlMostrarseAsync() => EjecutarAsync(async () =>
    {
        await Selector.CargarAsync();
        await CargarAsync();
    });

    /// <summary>Lee de nuevo los datos de la comisión elegida. Sin comisión, deja la pantalla vacía.</summary>
    public abstract Task CargarAsync();

    /// <summary>La comisión elegida o, si no hay ninguna, un aviso para el usuario.</summary>
    protected int ExigirComision() => Selector.ExigirComision();
}
