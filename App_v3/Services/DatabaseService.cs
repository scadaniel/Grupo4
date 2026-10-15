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
        await _dbConnection.CreateTableAsync<Asistencia>();
        await _dbConnection.CreateTableAsync<TrabajoPractico>();
        await _dbConnection.CreateTableAsync<EntregaTp>();

        // Cargar datos de prueba automáticamente si la base está limpia o vacía
        await InicializarDatosPruebaAsync();
    }

    public async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        await InitAsync();
        return _dbConnection!;
    }

    // --- SEED DATA (Datos Iniciales Automáticos) ---

    private async Task InicializarDatosPruebaAsync()
    {
        var cantidadEstudiantes = await _dbConnection!.Table<Estudiante>().CountAsync();
        if (cantidadEstudiantes == 0)
        {
            await _dbConnection.InsertAsync(new Estudiante { NombreCompleto = "Juan Pérez", Legajo = "1001", Dni = "40111222", Comision = "Comisión A", Activo = true });
            await _dbConnection.InsertAsync(new Estudiante { NombreCompleto = "María Gómez", Legajo = "1002", Dni = "40333444", Comision = "Comisión A", Activo = true });
            await _dbConnection.InsertAsync(new Estudiante { NombreCompleto = "Carlos Ruiz", Legajo = "1003", Dni = "40555666", Comision = "Comisión B", Activo = true });
        }

        var cantidadClases = await _dbConnection.Table<Clase>().CountAsync();
        if (cantidadClases == 0)
        {
            await _dbConnection.InsertAsync(new Clase
            {
                NombreMateria = "Robótica y Automatización",
                Comision = "Comisión A",
                NumeroClase = 1,
                Tema = "Introducción a Arduino y Sensores",
                Fecha = DateTime.Today.AddDays(-7)
            });

            await _dbConnection.InsertAsync(new Clase
            {
                NombreMateria = "Robótica y Automatización",
                Comision = "Comisión A",
                NumeroClase = 2,
                Tema = "Motores paso a paso y Puentes H",
                Fecha = DateTime.Today
            });
        }

        var cantidadTps = await _dbConnection.Table<TrabajoPractico>().CountAsync();
        if (cantidadTps == 0)
        {
            await _dbConnection.InsertAsync(new TrabajoPractico
            {
                Nombre = "TP 1: Semáforo Inteligente",
                Descripcion = "Montaje con LEDs y pulsadores",
                FechaLimite = DateTime.Today.AddDays(5)
            });
        }
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

    // --- MÓDULO CLASES (Unifica Clases y Comisiones) ---

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

    public async Task<List<Estudiante>> GetEstudiantesPorComisionAsync(string comision)
    {
        await InitAsync();
        if (string.IsNullOrWhiteSpace(comision))
        {
            return await GetEstudiantesAsync();
        }

        return await _dbConnection!.Table<Estudiante>()
            .Where(e => e.Activo && e.Comision == comision)
            .ToListAsync();
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

    // --- MÓDULO TRABAJOS PRÁCTICOS (H2) ---

    public async Task<List<TrabajoPractico>> GetTrabajosPracticosAsync()
    {
        await InitAsync();
        return await _dbConnection!.Table<TrabajoPractico>().ToListAsync();
    }

    public async Task<int> SaveTrabajoPracticoAsync(TrabajoPractico tp)
    {
        await InitAsync();
        if (tp.Id != 0)
        {
            return await _dbConnection!.UpdateAsync(tp);
        }
        else
        {
            return await _dbConnection!.InsertAsync(tp);
        }
    }

    public async Task<int> DeleteTrabajoPracticoAsync(TrabajoPractico tp)
    {
        await InitAsync();
        return await _dbConnection!.DeleteAsync(tp);
    }

    // --- MÓDULO ASISTENCIAS (H3 y H4) ---

    public async Task<List<Asistencia>> GetAsistenciasPorClaseAsync(int claseId)
    {
        await InitAsync();
        return await _dbConnection!.Table<Asistencia>()
            .Where(a => a.ClaseId == claseId)
            .ToListAsync();
    }

    public async Task GuardarListaAsistenciasAsync(int claseId, List<Asistencia> asistencias)
    {
        await InitAsync();

        foreach (var ast in asistencias)
        {
            ast.ClaseId = claseId;

            if (ast.Id != 0)
            {
                await _dbConnection!.UpdateAsync(ast);
            }
            else
            {
                await _dbConnection!.InsertAsync(ast);
            }

            // Recalcular inasistencias acumuladas del estudiante (Ausente y No Justificado)
            await RecalcularInasistenciasEstudianteAsync(ast.EstudianteId);
        }
    }

    private async Task RecalcularInasistenciasEstudianteAsync(int estudianteId)
    {
        var ausencias = await _dbConnection!.Table<Asistencia>()
            .Where(a => a.EstudianteId == estudianteId && !a.Presente && !a.Justificada)
            .CountAsync();

        var estudiante = await _dbConnection!.Table<Estudiante>()
            .FirstOrDefaultAsync(e => e.Id == estudianteId);

        if (estudiante != null)
        {
            estudiante.Inasistencias = ausencias;
            await _dbConnection!.UpdateAsync(estudiante);
        }
    }

    // --- MÓDULO ENTREGAS DE TRABAJOS PRÁCTICOS (H5) ---

    public async Task<List<EntregaTp>> GetEntregasPorTpAsync(int trabajoPracticoId)
    {
        await InitAsync();
        return await _dbConnection!.Table<EntregaTp>()
            .Where(e => e.TrabajoPracticoId == trabajoPracticoId)
            .ToListAsync();
    }

    public async Task GuardarListaEntregasTpAsync(int trabajoPracticoId, List<EntregaTp> entregas)
    {
        await InitAsync();

        foreach (var entrega in entregas)
        {
            entrega.TrabajoPracticoId = trabajoPracticoId;

            if (entrega.Id != 0)
            {
                await _dbConnection!.UpdateAsync(entrega);
            }
            else
            {
                await _dbConnection!.InsertAsync(entrega);
            }

            // Recalcular TPs no entregados del estudiante
            await RecalcularTpsNoEntregadosEstudianteAsync(entrega.EstudianteId);
        }
    }

    private async Task RecalcularTpsNoEntregadosEstudianteAsync(int estudianteId)
    {
        var tpsSinEntregar = await _dbConnection!.Table<EntregaTp>()
            .Where(e => e.EstudianteId == estudianteId && !e.Entregado)
            .CountAsync();

        var estudiante = await _dbConnection!.Table<Estudiante>()
            .FirstOrDefaultAsync(e => e.Id == estudianteId);

        if (estudiante != null)
        {
            estudiante.TpsNoEntregados = tpsSinEntregar;
            await _dbConnection!.UpdateAsync(estudiante);
        }
    }
}