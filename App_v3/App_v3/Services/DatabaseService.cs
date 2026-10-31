using SQLite;
using App_v3.Models;

namespace App_v3.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _dbConnection;

    /// <summary>
    /// Borra el archivo físico .db3 local y reinicia la conexión para reconstruir la base de datos de cero.
    /// </summary>
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

    /// <summary>
    /// Inicializa la conexión SQLite y crea automáticamente todas las tablas necesarias si no existen.
    /// </summary>
    private async Task InitAsync()
    {
        if (_dbConnection is not null)
            return;

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "EarlyWarningSystem.db3");

        _dbConnection = new SQLiteAsyncConnection(
            dbPath,
            SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache
        );

        // --- CREACIÓN DE TABLAS ---
        await _dbConnection.CreateTableAsync<Persona>();
        await _dbConnection.CreateTableAsync<Clase>();
        await _dbConnection.CreateTableAsync<Estudiante>();
        await _dbConnection.CreateTableAsync<Asistencia>();
        await _dbConnection.CreateTableAsync<TrabajoPractico>();
        await _dbConnection.CreateTableAsync<EntregaTp>();
        await _dbConnection.CreateTableAsync<Inscripcion>(); // Tabla intermedia para relación M:N (Alumnos <-> Clases)
        await _dbConnection.CreateTableAsync<Comision>(); //tabla comisiones

        // Cargar datos iniciales de prueba si las tablas están vacías
        await InicializarDatosPruebaAsync();
    }

    public async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        await InitAsync();
        return _dbConnection!;
    }

    // ==========================================
    // --- SEED DATA (Datos Iniciales) ---
    // ==========================================

    private async Task InicializarDatosPruebaAsync()
    {
        // Precargar Estudiantes de prueba
        var cantidadEstudiantes = await _dbConnection!.Table<Estudiante>().CountAsync();
        if (cantidadEstudiantes == 0)
        {
            await _dbConnection.InsertAsync(new Estudiante { NombreCompleto = "Juan Pérez", Legajo = "1001", Dni = "40111222", Comision = "Comisión A", Activo = true });
            await _dbConnection.InsertAsync(new Estudiante { NombreCompleto = "María Gómez", Legajo = "1002", Dni = "40333444", Comision = "Comisión A", Activo = true });
            await _dbConnection.InsertAsync(new Estudiante { NombreCompleto = "Carlos Ruiz", Legajo = "1003", Dni = "40555666", Comision = "Comisión B", Activo = true });
        }

        // Precargar Clases / Cátedras de prueba
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

            await _dbConnection.InsertAsync(new Clase
            {
                NombreMateria = "Robótica y Automatización",
                Comision = "Comisión B",
                NumeroClase = 1,
                Tema = "Introducción a Arduino y Sensores",
                Fecha = DateTime.Today.AddDays(-7)
            });
        }

        // Precargar Trabajos Prácticos de prueba
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

    // ==========================================
    // --- MÓDULO PERSONAS ---
    // ==========================================

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

    // ==========================================
    // --- MÓDULO CLASES Y CÁTEDRAS ---
    // ==========================================

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

    // ==========================================
    // --- MÓDULO ESTUDIANTES E INSCRIPCIONES ---
    // ==========================================

    public async Task<List<Estudiante>> GetEstudiantesAsync()
    {
        await InitAsync();
        return await _dbConnection!.Table<Estudiante>().ToListAsync();
    }

    /// <summary>
    /// Método legacy basado en texto de comisión (mantenido por compatibilidad).
    /// </summary>
    public async Task<List<Estudiante>> GetEstudiantesPorComisionAsync(string nombreComision)
    {
        await InitAsync();

        if (string.IsNullOrWhiteSpace(nombreComision))
            return new List<Estudiante>();

        string nombreLimpio = nombreComision.Trim();

        var estudiantes = await _dbConnection!.Table<Estudiante>().ToListAsync();
        return estudiantes
            .Where(e => !string.IsNullOrWhiteSpace(e.Comision) && e.Comision.Trim().Equals(nombreLimpio, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    /// <summary>
    /// Obtiene los estudiantes inscriptos a una clase específica mediante la tabla puente de inscripciones.
    /// </summary>
    public async Task<List<Estudiante>> GetEstudiantesPorClaseIdAsync(int claseId)
    {
        await InitAsync();

        var inscripciones = await _dbConnection!.Table<Inscripcion>()
            .Where(i => i.ClaseId == claseId)
            .ToListAsync();

        var estudianteIds = inscripciones.Select(i => i.EstudianteId).ToList();

        if (!estudianteIds.Any())
        {
            return new List<Estudiante>();
        }

        return await _dbConnection.Table<Estudiante>()
            .Where(e => estudianteIds.Contains(e.Id) && e.Activo)
            .ToListAsync();
    }

    public async Task<int> InscribirEstudianteAsync(int estudianteId, int claseId)
    {
        await InitAsync();

        var existente = await _dbConnection!.Table<Inscripcion>()
            .FirstOrDefaultAsync(i => i.EstudianteId == estudianteId && i.ClaseId == claseId);

        if (existente != null)
            return 0; // Ya se encuentra inscripto

        return await _dbConnection.InsertAsync(new Inscripcion
        {
            EstudianteId = estudianteId,
            ClaseId = claseId
        });
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

    // ==========================================
    // --- MÓDULO TRABAJOS PRÁCTICOS ---
    // ==========================================

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

    // ==========================================
    // --- MÓDULO ASISTENCIAS ---
    // ==========================================

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
            // Aseguramos que la asistencia guarde la fecha actual de la clase si no tiene una asignada
            if (ast.Fecha == default)
            {
                ast.Fecha = DateTime.Today;
            }

            if (ast.Id != 0)
            {
                await _dbConnection!.UpdateAsync(ast);
            }
            else
            {
                await _dbConnection!.InsertAsync(ast);
            }

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

    // ==========================================
    // --- MÓDULO ENTREGAS DE TPs ---
    // ==========================================

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

    /// <summary>
    /// Busca un estudiante por su DNI y lo inscribe en la clase especificada mediante la tabla puente.
    /// </summary>
    public async Task<bool> InscribirEstudiantePorDniAsync(string dni, int claseId)
    {
        await InitAsync();

        string dniLimpio = dni?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(dniLimpio))
            return false;

        var estudiante = await _dbConnection!.Table<Estudiante>()
            .FirstOrDefaultAsync(e => e.Dni == dniLimpio);

        if (estudiante == null)
        {
            System.Diagnostics.Debug.WriteLine($"[Error de inscripción] No se encontró alumno con DNI: '{dniLimpio}'");
            return false;
        }

        var existente = await _dbConnection.Table<Inscripcion>()
            .FirstOrDefaultAsync(i => i.EstudianteId == estudiante.Id && i.ClaseId == claseId);

        if (existente != null)
            return true;

        await _dbConnection.InsertAsync(new Inscripcion
        {
            EstudianteId = estudiante.Id,
            ClaseId = claseId
        });

        return true;
    }

    /// <summary>
    /// Obtiene el reporte de asistencias filtrado por DNI, Comisión y Rango de Fechas de forma segura.
    /// </summary>
    public async Task<List<ReporteAsistenciaItem>> ObtenerReporteAsistenciasAsync(string? dniFiltro, string? comisionFiltro, DateTime? fechaInicio, DateTime? fechaFin)
    {
        await InitAsync();

        var asistencias = await _dbConnection!.Table<Asistencia>().ToListAsync();
        var estudiantes = await _dbConnection.Table<Estudiante>().ToListAsync();
        var clases = await _dbConnection.Table<Clase>().ToListAsync();

        var listaCruda = from a in asistencias
                         join e in estudiantes on a.EstudianteId equals e.Id
                         join c in clases on a.ClaseId equals c.Id
                         select new ReporteAsistenciaItem
                         {
                             EstudianteId = e.Id,
                             ClaseId = c.Id,
                             NombreEstudiante = e.NombreCompleto ?? string.Empty,
                             Dni = e.Dni ?? string.Empty,
                             MateriaClase = $"{c.NombreMateria} (Clase #{c.NumeroClase}: {c.Tema})",
                             Comision = c.Comision ?? string.Empty,
                             // Prioriza la fecha propia de la asistencia; si no tiene, usa la de la clase
                             FechaClase = (a.Fecha != default(DateTime) && a.Fecha.Year > 2000) ? a.Fecha : c.Fecha,
                             Presente = a.Presente,
                             Justificada = a.Justificada
                         };

        var query = listaCruda.AsQueryable();

        // 1. Filtro por DNI
        if (!string.IsNullOrWhiteSpace(dniFiltro))
        {
            string dniTrim = dniFiltro.Trim();
            query = query.Where(x => !string.IsNullOrEmpty(x.Dni) && x.Dni.Contains(dniTrim, StringComparison.OrdinalIgnoreCase));
        }

        // 2. Filtro por Comisión
        if (!string.IsNullOrWhiteSpace(comisionFiltro) && comisionFiltro != "Todas las comisiones" && comisionFiltro != "General")
        {
            query = query.Where(x => !string.IsNullOrEmpty(x.Comision) && x.Comision.Trim().Equals(comisionFiltro.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        // 3. Filtro por Rango de Fechas
        if (fechaInicio.HasValue)
        {
            query = query.Where(x => x.FechaClase.Date >= fechaInicio.Value.Date);
        }

        if (fechaFin.HasValue)
        {
            query = query.Where(x => x.FechaClase.Date <= fechaFin.Value.Date);
        }

        return query.OrderByDescending(x => x.FechaClase).ToList();
    }

    public async Task<List<ReporteTpItem>> ObtenerReporteEntregasTpAsync(string? dniFiltro, int? trabajoPracticoIdFiltro)
    {
        await InitAsync();

        var entregas = await _dbConnection!.Table<EntregaTp>().ToListAsync();
        var estudiantes = await _dbConnection.Table<Estudiante>().ToListAsync();
        var tps = await _dbConnection.Table<TrabajoPractico>().ToListAsync();

        var listaCruda = from e in estudiantes.Where(x => x.Activo)
                         from tp in tps
                         join ent in entregas on new { EstudianteId = e.Id, TrabajoPracticoId = tp.Id }
                         equals new { ent.EstudianteId, ent.TrabajoPracticoId } into entregaJoin
                         from subEnt in entregaJoin.DefaultIfEmpty()
                         select new ReporteTpItem
                         {
                             EstudianteId = e.Id,
                             TrabajoPracticoId = tp.Id,
                             NombreEstudiante = e.NombreCompleto ?? string.Empty,
                             Dni = e.Dni ?? string.Empty,
                             NombreTp = tp.Nombre ?? string.Empty,
                             DescripcionTp = tp.Descripcion ?? string.Empty,
                             Entregado = subEnt != null && subEnt.Entregado,
                             FechaEntrega = subEnt?.FechaEntrega
                         };

        var query = listaCruda.AsQueryable();

        if (!string.IsNullOrWhiteSpace(dniFiltro))
        {
            string dniTrim = dniFiltro.Trim();
            query = query.Where(x => !string.IsNullOrEmpty(x.Dni) && x.Dni.Contains(dniTrim, StringComparison.OrdinalIgnoreCase));
        }

        if (trabajoPracticoIdFiltro.HasValue && trabajoPracticoIdFiltro.Value > 0)
        {
            query = query.Where(x => x.TrabajoPracticoId == trabajoPracticoIdFiltro.Value);
        }

        return query.OrderBy(x => x.NombreEstudiante).ThenBy(x => x.NombreTp).ToList();
    }

    // ==========================================
    // --- MÓDULO COMISIONES Y UMBRALES DE RIESGO ---
    // ==========================================

    public async Task<List<Comision>> GetComisionesAsync()
    {
        await InitAsync();
        return await _dbConnection!.Table<Comision>().ToListAsync();
    }

    public async Task<int> GuardarUmbralesComisionAsync(Comision comision)
    {
        await InitAsync();
        if (comision.Id != 0)
        {
            return await _dbConnection!.UpdateAsync(comision);
        }
        else
        {
            return await _dbConnection!.InsertAsync(comision);
        }
    }

    public async Task<int> InscribirEstudianteEnComisionAsync(int estudianteId, int comisionId)
    {
        await InitAsync();

        var estudiante = await _dbConnection!.Table<Estudiante>().FirstOrDefaultAsync(e => e.Id == estudianteId);
        var comision = await _dbConnection!.Table<Comision>().FirstOrDefaultAsync(c => c.Id == comisionId);

        if (estudiante != null && comision != null)
        {
            estudiante.Comision = comision.Nombre;
            return await _dbConnection.UpdateAsync(estudiante);
        }
        return 0;
    }

    public async Task<int> SaveClaseYComisionNuevaAsync(Clase clase)
    {
        await InitAsync();

        if (!string.IsNullOrWhiteSpace(clase.Comision))
        {
            string nombreLimpio = clase.Comision.Trim();
            var comisionExistente = await _dbConnection!.Table<Comision>()
                .FirstOrDefaultAsync(c => c.Nombre == nombreLimpio);

            if (comisionExistente == null)
            {
                await _dbConnection.InsertAsync(new Comision { Nombre = nombreLimpio });
            }
        }

        if (clase.Id != 0)
        {
            return await _dbConnection!.UpdateAsync(clase);
        }
        else
        {
            return await _dbConnection!.InsertAsync(clase);
        }
    }
}