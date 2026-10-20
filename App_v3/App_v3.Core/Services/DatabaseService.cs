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

            await db.CreateTablesAsync(
                CreateFlags.None,
                typeof(Persona),
                typeof(Clase),
                typeof(ConfiguracionRiesgo),
                typeof(Asistencia),
                typeof(TrabajoPractico),
                typeof(EntregaTp),
                typeof(Inscripcion));

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
                new() { Nombre = "Martín", Apellido = "González", Legajo = "L-1001", Dni = "40111001", Comision = "ComisiónA", Email = "martin.gonzalez@ejemplo.com", Telefono = "11-5555-0101", EstadoAlerta = "Normal" },
                new() { Nombre = "Sofía", Apellido = "Rodríguez", Legajo = "L-1002", Dni = "40111002", Comision = "ComisiónA", Email = "sofia.rodriguez@ejemplo.com", Telefono = "11-5555-0102", EstadoAlerta = "Normal" },
                new() { Nombre = "Lucas", Apellido = "Fernández", Legajo = "L-1003", Dni = "40111003", Comision = "ComisiónB", Email = "lucas.fernandez@ejemplo.com", Telefono = "11-5555-0103", EstadoAlerta = "Riesgo Moderado" },
                new() { Nombre = "Valentina", Apellido = "López", Legajo = "L-1004", Dni = "40111004", Comision = "ComisiónA", Email = "valentina.lopez@ejemplo.com", Telefono = "11-5555-0104", EstadoAlerta = "Normal" },
                new() { Nombre = "Joaquín", Apellido = "Martínez", Legajo = "L-1005", Dni = "40111005", Comision = "Comisión B", Email = "joaquin.martinez@ejemplo.com", Telefono = "11-5555-0105", EstadoAlerta = "Riesgo Alto" },
                new() { Nombre = "Camila", Apellido = "Sánchez", Legajo = "L-1006", Dni = "40111006", Comision = "Comisión A", Email = "camila.sanchez@ejemplo.com", Telefono = "11-5555-0106", EstadoAlerta = "Normal" },
                new() { Nombre = "Tomás", Apellido = "Díaz", Legajo = "L-1007", Dni = "40111007", Comision = "Comisión B", Email = "tomas.diaz@ejemplo.com", Telefono = "11-5555-0107", EstadoAlerta = "Riesgo Moderado" },
                new() { Nombre = "Agustina", Apellido = "Romero", Legajo = "L-1008", Dni = "40111008", Comision = "ComisiónB", Email = "agustina.romero@ejemplo.com", Telefono = "11-5555-0108", EstadoAlerta = "Normal" }
            };
            await db.InsertAllAsync(estudiantes);
        }

        if (await db.Table<Clase>().CountAsync() == 0)
        {
            var clases = new List<Clase>
            {
                new() { NombreMateria = "Robótica y Automatización", Comision = "Comisión A", NumeroClase = 1, Tema = "Introducción a Arduino y Sensores", Fecha = DateTime.Today.AddDays(-7) },
                new() { NombreMateria = "Robótica y Automatización", Comision = "Comisión A", NumeroClase = 2, Tema = "Motores paso a paso y Puentes H", Fecha = DateTime.Today },
                new() { NombreMateria = "Robótica y Automatización", Comision = "Comisión B", NumeroClase = 1, Tema = "Introducción a Arduino y Sensores", Fecha = DateTime.Today.AddDays(-7) }
            };
            await db.InsertAllAsync(clases);
        }

        if (await db.Table<TrabajoPractico>().CountAsync() == 0)
        {
            await db.InsertAsync(new TrabajoPractico
            {
                Nombre = "TP 1: Semáforo Inteligente",
                Descripcion = "Montaje con LEDs y pulsadores",
                FechaLimite = DateTime.Today.AddDays(5)
            });
        }
    }

    // ==========================================
    // --- GENÉRICOS ---
    // ==========================================

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
            
            // Breve respiro para liberar el archivo a nivel SO
            await Task.Delay(150);

            if (File.Exists(_dbPath))
            {
                try
                {
                    File.Delete(_dbPath);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[Error al borrar BD]: {ex.Message}");
                }
            }
        }
        finally
        {
            _initLock.Release();
        }

        // Si querés que el reset devuelva la app con los datos iniciales de fábrica, cambiá a true:
        await InitAsync(sembrar: true);
    }

    // ==========================================
    // --- CLASES Y TRABAJOS PRÁCTICOS ---
    // ==========================================

    public Task<List<Clase>> GetClasesAsync() => GetAllAsync<Clase>();

    public Task<int> SaveClaseAsync(Clase clase) => SaveAsync(clase);

    public Task<List<TrabajoPractico>> GetTrabajosPracticosAsync() => GetAllAsync<TrabajoPractico>();

    public Task<int> SaveTrabajoPracticoAsync(TrabajoPractico tp) => SaveAsync(tp);

    // ==========================================
    // --- ESTUDIANTES E INSCRIPCIONES ---
    // ==========================================

    public async Task<List<Persona>> GetEstudiantesPorComisionAsync(string comision)
    {
        var db = await InitAsync();
        var personas = await db.Table<Persona>().ToListAsync();

        var estudiantes = personas.Where(p => p.Rol == RolPersona.Estudiante && p.Activo);

        if (!string.IsNullOrWhiteSpace(comision))
            estudiantes = estudiantes.Where(p => p.Comision == comision);

        return estudiantes.ToList();
    }

    public async Task<List<Persona>> GetEstudiantesPorClaseIdAsync(int claseId)
    {
        var db = await InitAsync();

        var inscripciones = await db.Table<Inscripcion>()
            .Where(i => i.ClaseId == claseId)
            .ToListAsync();

        var ids = inscripciones.Select(i => i.EstudianteId).ToHashSet();
        if (ids.Count == 0)
            return new List<Persona>();

        var personas = await db.Table<Persona>().ToListAsync();
        return personas
            .Where(p => ids.Contains(p.Id) && p.Rol == RolPersona.Estudiante && p.Activo)
            .ToList();
    }

    public async Task<bool> InscribirEstudiantePorDniAsync(string dni, int claseId)
    {
        var db = await InitAsync();

        string dniLimpio = dni?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(dniLimpio))
            return false;

        var personas = await db.Table<Persona>().ToListAsync();
        var estudiante = personas.FirstOrDefault(p => p.Rol == RolPersona.Estudiante && p.Dni == dniLimpio);

        if (estudiante is null)
        {
            System.Diagnostics.Debug.WriteLine($"[Error de inscripción] No se encontró alumno con DNI: '{dniLimpio}'");
            return false;
        }

        var existente = await db.Table<Inscripcion>()
            .FirstOrDefaultAsync(i => i.EstudianteId == estudiante.Id && i.ClaseId == claseId);

        if (existente is not null)
            return true;

        await db.InsertAsync(new Inscripcion { EstudianteId = estudiante.Id, ClaseId = claseId });
        return true;
    }

    // ==========================================
    // --- ASISTENCIAS ---
    // ==========================================

    public async Task<List<Asistencia>> GetAsistenciasPorClaseAsync(int claseId)
    {
        var db = await InitAsync();
        return await db.Table<Asistencia>().Where(a => a.ClaseId == claseId).ToListAsync();
    }

    public async Task GuardarListaAsistenciasAsync(int claseId, List<Asistencia> asistencias)
    {
        var db = await InitAsync();

        foreach (var ast in asistencias)
        {
            ast.ClaseId = claseId;

            if (ast.Id != 0)
                await db.UpdateAsync(ast);
            else
                await db.InsertAsync(ast);
        }

        foreach (var estudianteId in asistencias.Select(a => a.EstudianteId).Distinct())
            await RecalcularInasistenciasAsync(db, estudianteId);
    }

    private static async Task RecalcularInasistenciasAsync(SQLiteAsyncConnection db, int estudianteId)
    {
        var ausencias = await db.Table<Asistencia>()
            .Where(a => a.EstudianteId == estudianteId && !a.Presente && !a.Justificada)
            .CountAsync();

        var persona = await db.Table<Persona>().FirstOrDefaultAsync(p => p.Id == estudianteId);
        if (persona is not null)
        {
            persona.Inasistencias = ausencias;
            await db.UpdateAsync(persona);
        }
    }

    // ==========================================
    // --- ENTREGAS DE TPs ---
    // ==========================================

    public async Task<List<EntregaTp>> GetEntregasPorTpAsync(int trabajoPracticoId)
    {
        var db = await InitAsync();
        return await db.Table<EntregaTp>()
            .Where(e => e.TrabajoPracticoId == trabajoPracticoId)
            .ToListAsync();
    }

    public async Task GuardarListaEntregasTpAsync(int trabajoPracticoId, List<EntregaTp> entregas)
    {
        var db = await InitAsync();

        foreach (var entrega in entregas)
        {
            entrega.TrabajoPracticoId = trabajoPracticoId;

            if (entrega.Id != 0)
                await db.UpdateAsync(entrega);
            else
                await db.InsertAsync(entrega);
        }

        foreach (var estudianteId in entregas.Select(e => e.EstudianteId).Distinct())
            await RecalcularTpsNoEntregadosAsync(db, estudianteId);
    }

    private static async Task RecalcularTpsNoEntregadosAsync(SQLiteAsyncConnection db, int estudianteId)
    {
        var sinEntregar = await db.Table<EntregaTp>()
            .Where(e => e.EstudianteId == estudianteId && !e.Entregado)
            .CountAsync();

        var persona = await db.Table<Persona>().FirstOrDefaultAsync(p => p.Id == estudianteId);
        if (persona is not null)
        {
            persona.TpsNoEntregados = sinEntregar;
            await db.UpdateAsync(persona);
        }
    }

    // ==========================================
    // --- REPORTES ---
    // ==========================================

    public async Task<List<ReporteAsistenciaItem>> ObtenerReporteAsistenciasAsync(
        string? dniFiltro, int? claseIdFiltro, DateTime? fechaInicio, DateTime? fechaFin)
    {
        var db = await InitAsync();

        var asistencias = await db.Table<Asistencia>().ToListAsync();
        var estudiantes = (await db.Table<Persona>().ToListAsync())
            .Where(p => p.Rol == RolPersona.Estudiante).ToList();
        var clases = await db.Table<Clase>().ToListAsync();

        var query = from a in asistencias
                    join e in estudiantes on a.EstudianteId equals e.Id
                    join c in clases on a.ClaseId equals c.Id
                    select new ReporteAsistenciaItem
                    {
                        EstudianteId = e.Id,
                        ClaseId = c.Id,
                        NombreEstudiante = e.NombreCompleto,
                        Dni = e.Dni ?? string.Empty,
                        MateriaClase = $"{c.NombreMateria} (Clase #{c.NumeroClase}: {c.Tema})",
                        Comision = c.Comision ?? string.Empty,
                        FechaClase = c.Fecha,
                        Presente = a.Presente,
                        Justificada = a.Justificada
                    };

        if (!string.IsNullOrWhiteSpace(dniFiltro))
        {
            string dniTrim = dniFiltro.Trim();
            query = query.Where(x => x.Dni.Contains(dniTrim, StringComparison.OrdinalIgnoreCase));
        }

        if (claseIdFiltro.HasValue && claseIdFiltro.Value > 0)
            query = query.Where(x => x.ClaseId == claseIdFiltro.Value);

        if (fechaInicio.HasValue)
            query = query.Where(x => x.FechaClase.Date >= fechaInicio.Value.Date);

        if (fechaFin.HasValue)
            query = query.Where(x => x.FechaClase.Date <= fechaFin.Value.Date);

        return query.OrderByDescending(x => x.FechaClase).ToList();
    }

    public async Task<List<ReporteTpItem>> ObtenerReporteEntregasTpAsync(
        string? dniFiltro, int? trabajoPracticoIdFiltro)
    {
        var db = await InitAsync();

        var entregas = await db.Table<EntregaTp>().ToListAsync();
        var estudiantes = (await db.Table<Persona>().ToListAsync())
            .Where(p => p.Rol == RolPersona.Estudiante && p.Activo).ToList();
        var tps = await db.Table<TrabajoPractico>().ToListAsync();

        var query = from e in estudiantes
                    from tp in tps
                    join ent in entregas
                        on new { EstudianteId = e.Id, TrabajoPracticoId = tp.Id }
                        equals new { ent.EstudianteId, ent.TrabajoPracticoId } into entregaJoin
                    from subEnt in entregaJoin.DefaultIfEmpty()
                    select new ReporteTpItem
                    {
                        EstudianteId = e.Id,
                        TrabajoPracticoId = tp.Id,
                        NombreEstudiante = e.NombreCompleto,
                        Dni = e.Dni ?? string.Empty,
                        NombreTp = tp.Nombre ?? string.Empty,
                        DescripcionTp = tp.Descripcion ?? string.Empty,
                        Entregado = subEnt != null && subEnt.Entregado,
                        FechaEntrega = subEnt?.FechaEntrega
                    };

        if (!string.IsNullOrWhiteSpace(dniFiltro))
        {
            string dniTrim = dniFiltro.Trim();
            query = query.Where(x => x.Dni.Contains(dniTrim, StringComparison.OrdinalIgnoreCase));
        }

        if (trabajoPracticoIdFiltro.HasValue && trabajoPracticoIdFiltro.Value > 0)
            query = query.Where(x => x.TrabajoPracticoId == trabajoPracticoIdFiltro.Value);

        return query.OrderBy(x => x.NombreEstudiante).ThenBy(x => x.NombreTp).ToList();
    }
}