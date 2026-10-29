using SQLite;
using App_v3.Core;

namespace App_v3.Services;

// Fila de la tabla Contacto. Es la versión de la base de datos del record Contacto del dominio.
// Se mantiene acá, en la app, para que el dominio no dependa de SQLite.
[Table("Contacto")]
public class ContactoRegistro
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int EstudianteId { get; set; }

    public DateTime Fecha { get; set; }

    public MedioContacto Medio { get; set; }

    public string Resultado { get; set; } = string.Empty;

    public string Nota { get; set; } = string.Empty;
}

// Guarda y lee los contactos usando la misma base de datos que el resto de la app.
public class ContactoStoreSqlite(DatabaseService db) : IContactoStore
{
    public async Task GuardarAsync(Contacto contacto)
    {
        var conexion = await ObtenerConexionAsync();

        await conexion.InsertAsync(new ContactoRegistro
        {
            EstudianteId = contacto.EstudianteId,
            Fecha = contacto.Fecha,
            Medio = contacto.Medio,
            Resultado = contacto.Resultado,
            Nota = contacto.Nota
        });
    }

    public async Task<IReadOnlyList<Contacto>> ListarAsync()
    {
        var conexion = await ObtenerConexionAsync();
        var filas = await conexion.Table<ContactoRegistro>().ToListAsync();

        return filas
            .Select(f => new Contacto(f.EstudianteId, f.Fecha, f.Medio, f.Resultado, f.Nota))
            .ToList();
    }

    // La tabla se crea si no existe. Así también funciona después de "Borrar BD",
    // que elimina el archivo de la base y lo arma de nuevo.
    private async Task<SQLiteAsyncConnection> ObtenerConexionAsync()
    {
        var conexion = await db.GetConnectionAsync();
        await conexion.CreateTableAsync<ContactoRegistro>();
        return conexion;
    }
}
