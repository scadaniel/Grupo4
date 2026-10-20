using SQLite;
using App_v3.Models;

namespace App_v3.Services;

public class DatabaseService : IDatabaseService
{
    private readonly string _dbPath;
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private SQLiteAsyncConnection? _db;

    public DatabaseService(string dbPath) => _dbPath = dbPath;

    private async Task<SQLiteAsyncConnection> InitAsync(bool sembrar = true)
    {
        if (_db is not null) return _db;

        await _initLock.WaitAsync();
        try
        {
            if (_db is not null) return _db;

            var db = new SQLiteAsyncConnection(
                _dbPath,
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);

            await db.CreateTablesAsync<Persona, Clase, ConfiguracionRiesgo>();
            if (sembrar) await SembrarDatosAsync(db);
            _db = db;
            return _db;
        }
        finally
        {
            _initLock.Release();
        }
    }

    // Datos de ejemplo: solo se cargan si las tablas están vacías
    private static async Task SembrarDatosAsync(SQLiteAsyncConnection db)
    {
        if (await db.Table<Persona>().CountAsync() == 0)
        {
            var estudiantes = new List<Persona>
            {
                new() { Nombre = "Martín", Apellido = "González", Legajo = "L-1001", Email = "martin.gonzalez@ejemplo.com", Telefono = "11-5555-0101", EstadoAlerta = "Normal" },
                new() { Nombre = "Sofía", Apellido = "Rodríguez", Legajo = "L-1002", Email = "sofia.rodriguez@ejemplo.com", Telefono = "11-5555-0102", EstadoAlerta = "Normal" },
                new() { Nombre = "Lucas", Apellido = "Fernández", Legajo = "L-1003", Email = "lucas.fernandez@ejemplo.com", Telefono = "11-5555-0103", EstadoAlerta = "Riesgo Moderado" },
                new() { Nombre = "Valentina", Apellido = "López", Legajo = "L-1004", Email = "valentina.lopez@ejemplo.com", Telefono = "11-5555-0104", EstadoAlerta = "Normal" },
                new() { Nombre = "Joaquín", Apellido = "Martínez", Legajo = "L-1005", Email = "joaquin.martinez@ejemplo.com", Telefono = "11-5555-0105", EstadoAlerta = "Riesgo Alto" },
                new() { Nombre = "Camila", Apellido = "Sánchez", Legajo = "L-1006", Email = "camila.sanchez@ejemplo.com", Telefono = "11-5555-0106", EstadoAlerta = "Normal" },
                new() { Nombre = "Tomás", Apellido = "Díaz", Legajo = "L-1007", Email = "tomas.diaz@ejemplo.com", Telefono = "11-5555-0107", EstadoAlerta = "Riesgo Moderado" },
                new() { Nombre = "Agustina", Apellido = "Romero", Legajo = "L-1008", Email = "agustina.romero@ejemplo.com", Telefono = "11-5555-0108", EstadoAlerta = "Normal" }
            };
            await db.InsertAllAsync(estudiantes);
        }

        if (await db.Table<Clase>().CountAsync() == 0)
        {
            var clases = new List<Clase>
            {
                new() { NombreMateria = "Programación III", Comision = "Comisión A" },
                new() { NombreMateria = "Base de Datos", Comision = "Comisión B" },
                new() { NombreMateria = "Ingeniería de Software", Comision = "Comisión A" }
            };
            await db.InsertAllAsync(clases);
        }
    }
    public async Task<List<T>> GetAllAsync<T>() where T : class, new()
        => await (await InitAsync()).Table<T>().ToListAsync();

    public async Task<int> SaveAsync<T>(T item) where T : class, IEntidad, new()
    {
        var db = await InitAsync();
        return item.Id != 0 ? await db.UpdateAsync(item) : await db.InsertAsync(item);
    }

    public async Task<int> DeleteAsync<T>(T item) where T : class, new()
        => await (await InitAsync()).DeleteAsync(item);

    public async Task ResetAsync()
    {
        await _initLock.WaitAsync();
        try
        {
            if (_db is not null)
            {
                await _db.CloseAsync();
                _db = null;
            }

            SQLiteAsyncConnection.ResetPool();

            if (File.Exists(_dbPath))
                File.Delete(_dbPath);
        }
        finally
        {
            _initLock.Release();
        }

        await InitAsync(sembrar: false);
    }
}