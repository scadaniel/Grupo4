using App_v3.Dominio;
using SQLite;

namespace App_v3.Plataforma;

/// <summary>
/// Guarda las entidades del dominio en un archivo SQLite. Es lo único de la app que conoce SQLite:
/// las entidades no llevan atributos, la clave "Id" se declara acá con ImplicitPK + AutoIncPK.
/// </summary>
public sealed class RepositorioSqlite(string rutaArchivo) : IRepositorio
{
    private const CreateFlags Banderas = CreateFlags.ImplicitPK | CreateFlags.AutoIncPK;

    private static readonly Type[] Tablas =
    [
        typeof(Materia), typeof(Comision), typeof(Estudiante), typeof(Inscripcion),
        typeof(ClaseDictada), typeof(TrabajoPractico), typeof(Asistencia),
        typeof(Entrega), typeof(Contacto), typeof(UmbralesRiesgo)
    ];

    private readonly SQLiteAsyncConnection _conexion = new(rutaArchivo);
    private readonly SemaphoreSlim _candado = new(1, 1);
    private bool _tablasCreadas;

    public async Task<List<T>> ListarAsync<T>() where T : class, IEntidad, new()
    {
        await AsegurarTablasAsync();
        return await _conexion.Table<T>().ToListAsync();
    }

    public async Task GuardarAsync<T>(T entidad) where T : class, IEntidad, new()
    {
        await AsegurarTablasAsync();
        if (entidad.Id == 0)
            await _conexion.InsertAsync(entidad);
        else
            await _conexion.InsertOrReplaceAsync(entidad);
    }

    public async Task VaciarAsync()
    {
        await AsegurarTablasAsync();
        foreach (var tipo in Tablas)
        {
            var tabla = await _conexion.GetMappingAsync(tipo, Banderas);
            await _conexion.ExecuteAsync($"DELETE FROM \"{tabla.TableName}\"");
        }
    }

    private async Task AsegurarTablasAsync()
    {
        if (_tablasCreadas) return;

        await _candado.WaitAsync();
        try
        {
            if (_tablasCreadas) return;

            foreach (var tipo in Tablas)
                await _conexion.CreateTableAsync(tipo, Banderas);
            _tablasCreadas = true;
        }
        finally
        {
            _candado.Release();
        }
    }
}
