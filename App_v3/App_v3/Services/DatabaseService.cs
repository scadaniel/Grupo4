using SQLite;
using App_v3.Models;

namespace App_v3.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _database;

    public async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        if (_database is not null)
            return _database;

        // Inicializador explícito del proveedor nativo de SQLite
        SQLitePCL.Batteries_V2.Init();

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "alerta_temprana.db3");
        _database = new SQLiteAsyncConnection(dbPath);

        // Creación de las tablas de la base de datos
        await _database.CreateTableAsync<Estudiante>();
        await _database.CreateTableAsync<Profesor>();
        await _database.CreateTableAsync<Clase>();
        await _database.CreateTableAsync<Asistencia>();
        await _database.CreateTableAsync<TrabajoPractico>();
        await _database.CreateTableAsync<EntregaTrabajo>();

        return _database;
    }
}