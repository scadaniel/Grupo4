namespace App_v3.Dominio;

/// <summary>
/// Lo único que el dominio necesita saber de la persistencia.
/// El dominio no conoce SQLite: la app lo implementa con la base de datos
/// y las pruebas con una lista en memoria.
/// </summary>
public interface IRepositorio
{
    Task<List<T>> ListarAsync<T>() where T : class, IEntidad;

    /// <summary>Si Id es 0 inserta y completa el Id; si no, reemplaza el registro con ese Id.</summary>
    Task GuardarAsync<T>(T entidad) where T : class, IEntidad;
}
