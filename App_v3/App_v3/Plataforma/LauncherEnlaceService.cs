using App_v3.ViewModels;

namespace App_v3.Plataforma;

/// <summary>Abre mailto: y tel: con la aplicación del sistema. Si no hay ninguna, devuelve false en lugar de fallar.</summary>
public sealed class LauncherEnlaceService : IEnlaceService
{
    public async Task<bool> AbrirAsync(string direccion)
    {
        try
        {
            var uri = new Uri(direccion);
            return await Launcher.Default.CanOpenAsync(uri) && await Launcher.Default.OpenAsync(uri);
        }
        catch (Exception)
        {
            return false;
        }
    }
}
