namespace App_v3.ViewModels;

/// <summary>Mensajes al usuario. La implementación real usa las alertas de Shell; las pruebas usan una falsa.</summary>
public interface IDialogService
{
    Task AlertAsync(string titulo, string mensaje);

    /// <summary>Pregunta Aceptar / Cancelar. Devuelve true si el usuario acepta.</summary>
    Task<bool> ConfirmarAsync(string titulo, string mensaje);
}

/// <summary>Navegación entre páginas, sin que los ViewModels conozcan Shell.</summary>
public interface INavegacionService
{
    Task IrAAsync(string ruta);
    Task VolverAsync();
}

/// <summary>Abre un enlace del sistema (mailto:, tel:). Devuelve false si no hay aplicación que lo abra.</summary>
public interface IEnlaceService
{
    Task<bool> AbrirAsync(string direccion);
}

/// <summary>Una pantalla que vuelve a leer sus datos cada vez que aparece (pueden haber cambiado en otra pestaña).</summary>
public interface IPantalla
{
    Task AlMostrarseAsync();
}

/// <summary>Nombres de las rutas de las páginas que no son pestañas. Los usan AppShell y los ViewModels.</summary>
public static class Rutas
{
    public const string ReporteAsistencias = "ReporteAsistencias";
    public const string ReporteEntregas = "ReporteEntregas";
    public const string Reportes = "Reportes";
    public const string Configuracion = "Configuracion";
}
