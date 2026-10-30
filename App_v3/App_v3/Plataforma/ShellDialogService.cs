using App_v3.ViewModels;

namespace App_v3.Plataforma;

/// <summary>Muestra los mensajes con las alertas de la pantalla actual de Shell.</summary>
public sealed class ShellDialogService : IDialogService
{
    public async Task AlertAsync(string titulo, string mensaje)
    {
        if (Shell.Current is { } shell)
            await shell.DisplayAlertAsync(titulo, mensaje, "Aceptar");
    }

    public async Task<bool> ConfirmarAsync(string titulo, string mensaje) =>
        Shell.Current is { } shell && await shell.DisplayAlertAsync(titulo, mensaje, "Aceptar", "Cancelar");
}
