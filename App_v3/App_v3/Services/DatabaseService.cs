using SQLite;
using App_v3.Models;

namespace App_v3.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _dbConnection;

    // Método que borra el archivo físico .db3 y reconstruye la base de datos de cero
    public async Task ResetDatabaseAsync()
    {
        if (_dbConnection is not null)
        {
            await _dbConnection.CloseAsync();
            _dbConnection = null;
        }

        SQLiteAsyncConnection.ResetPool();

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "EarlyWarningSystem.db3");
        if (File.Exists(dbPath))
        {
            File.Delete(dbPath);
        }

        await InitAsync();
    }

    private async Task InitAsync()
    {
        if (_dbConnection is not null)
            return;

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "EarlyWarningSystem.db3");

        _dbConnection = new SQLiteAsyncConnection(
            dbPath,
            SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache
        );

        // Crea las tablas si no existen con el esquema C# correcto
        await _dbConnection.CreateTableAsync<Persona>();
        await _dbConnection.CreateTableAsync<Clase>();
        await _dbConnection.CreateTableAsync<Estudiante>();
    }

    public async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        await InitAsync();
        return _dbConnection!;
    }

    // --- MÓDULO PERSONAS ---

    public async Task<List<Persona>> GetPersonasAsync()
    {
        await InitAsync();
        return await _dbConnection!.Table<Persona>().ToListAsync();
    }

    public async Task<int> SavePersonaAsync(Persona persona)
    {
        await InitAsync();
        if (persona.Id != 0)
        {
            return await _dbConnection!.UpdateAsync(persona);
        }
        else
        {
            return await _dbConnection!.InsertAsync(persona);
        }
    }

    public async Task<int> DeletePersonaAsync(Persona persona)
    {
        await InitAsync();
        return await _dbConnection!.DeleteAsync(persona);
    }

    // --- MÓDULO CLASES ---

    public async Task<List<Clase>> GetClasesAsync()
    {
        await InitAsync();
        return await _dbConnection!.Table<Clase>().ToListAsync();
    }

    public async Task<int> SaveClaseAsync(Clase clase)
    {
        await InitAsync();
        if (clase.Id != 0)
        {
            return await _dbConnection!.UpdateAsync(clase);
        }
        else
        {
            return await _dbConnection!.InsertAsync(clase);
        }
    }

    public async Task<int> DeleteClaseAsync(Clase clase)
    {
        await InitAsync();
        return await _dbConnection!.DeleteAsync(clase);
    }

    // --- MÓDULO ESTUDIANTES ---

    public async Task<List<Estudiante>> GetEstudiantesAsync()
    {
        await InitAsync();
        return await _dbConnection!.Table<Estudiante>().ToListAsync();
    }

    public async Task<int> SaveEstudianteAsync(Estudiante estudiante)
    {
        await InitAsync();
        if (estudiante.Id != 0)
        {
            return await _dbConnection!.UpdateAsync(estudiante);
        }
        else
        {
            return await _dbConnection!.InsertAsync(estudiante);
        }
    }

    public async Task<int> DeleteEstudianteAsync(Estudiante estudiante)
    {
        await InitAsync();
        return await _dbConnection!.DeleteAsync(estudiante);
    }
}