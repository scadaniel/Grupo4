using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using App_v3.Dominio;

namespace App_v3.ViewModels;

/// <summary>Avisa a la pantalla cuando cambia una propiedad. Base de los ViewModels y de las filas de las listas.</summary>
public abstract class NotificadorBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propiedad = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propiedad));

    protected bool SetProperty<T>(ref T campo, T valor, [CallerMemberName] string? propiedad = null)
    {
        if (EqualityComparer<T>.Default.Equals(campo, valor)) return false;

        campo = valor;
        OnPropertyChanged(propiedad);
        return true;
    }
}

/// <summary>
/// Base de las pantallas: además de avisar cambios, muestra los errores de negocio
/// como mensajes en vez de dejar que cierren la app.
/// </summary>
public abstract class BaseViewModel(IDialogService dialogos) : NotificadorBase
{
    protected IDialogService Dialogos { get; } = dialogos;

    /// <summary>Crea un comando que muestra los errores en un mensaje.</summary>
    protected ICommand Comando(Func<Task> accion) => new ComandoAsync(() => EjecutarAsync(accion));

    /// <summary>Ejecuta la acción; una regla de negocio rota se muestra como "Atención", cualquier otro error como "Error".</summary>
    protected async Task EjecutarAsync(Func<Task> accion)
    {
        try
        {
            await accion();
        }
        catch (ReglaDeNegocioException ex)
        {
            await Dialogos.AlertAsync("Atención", ex.Message);
        }
        catch (Exception ex)
        {
            await Dialogos.AlertAsync("Error", $"Ocurrió un error inesperado: {ex.Message}");
        }
    }
}
