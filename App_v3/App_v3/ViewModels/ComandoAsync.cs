using System.Windows.Input;

namespace App_v3.ViewModels;

/// <summary>
/// Comando que ejecuta una acción asíncrona y no se deja ejecutar de nuevo mientras la anterior no terminó
/// (evita guardar dos veces por un doble toque).
/// </summary>
public sealed class ComandoAsync(Func<Task> accion) : ICommand
{
    private bool _ejecutando;

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => !_ejecutando;

    public async void Execute(object? parameter) => await EjecutarAsync();

    public async Task EjecutarAsync()
    {
        if (_ejecutando) return;

        Cambiar(ejecutando: true);
        try
        {
            await accion();
        }
        finally
        {
            Cambiar(ejecutando: false);
        }
    }

    private void Cambiar(bool ejecutando)
    {
        _ejecutando = ejecutando;
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
