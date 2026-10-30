using App_v3.ViewModels;

namespace App_v3.Plataforma;

/// <summary>Navega entre páginas con las rutas registradas en AppShell.</summary>
public sealed class ShellNavegacionService : INavegacionService
{
    public Task IrAAsync(string ruta) => Shell.Current.GoToAsync(ruta);

    public Task VolverAsync() => Shell.Current.GoToAsync("..");
}
