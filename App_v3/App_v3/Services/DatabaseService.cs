using SQLite;
using App_v3.Models;
using System.IO;

namespace App_v3.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _dbConnection;

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

        // --- CREACIÓN DE TABLAS ---
        await _dbConnection.CreateTableAsync<Persona>();
        await _dbConnection.CreateTableAsync<Clase>();
        await _dbConnection.CreateTableAsync<Estudiante>();
        await _dbConnection.CreateTableAsync<Asistencia>();
        await _dbConnection.CreateTableAsync<TrabajoPractico>();
        await _dbConnection.CreateTableAsync<EntregaTp>();
        await _dbConnection.CreateTableAsync<Inscripcion>();
        await _dbConnection.CreateTableAsync<Comision>();
        await _dbConnection.CreateTableAsync<EstudianteComision>();
        await _dbConnection.CreateTableAsync<IntervencionContacto>();

        await InicializarDatosPruebaAsync();
    }

    public async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        await InitAsync();
        return _dbConnection!;
    }

    private async Task InicializarDatosPruebaAsync()
    {
        // Comisiones base
        var cantidadComisiones = await _dbConnection!.Table<Comision>().CountAsync();
        if (cantidadComisiones == 0)
        {
            await _dbConnection.InsertAsync(new Comision
            {
                Nombre = "Comisión A",
                InasistenciasBajo = 2,
                InasistenciasMedio = 4,
                TpsBajo = 1,
                TpsMedio = 2
            });
            await _dbConnection.InsertAsync(new Comision
            {
                Nombre = "Comisión B",
                InasistenciasBajo = 2,
                InasistenciasMedio = 4,
                TpsBajo = 1,
                TpsMedio = 2
            });
        }

        var cantidadEstudiantes = await _dbConnection.Table<Estudiante>().CountAsync();
        if (cantidadEstudiantes == 0)
        {
            await _dbConnection.InsertAsync(new Estudiante
            {
                NombreCompleto = "Juan Pérez",
                Legajo = "1001",
                Dni = "40111222",
                Comision = "Comisión A",
                Activo = true,
                Telefono = "+5491111111111",
                Email = "juan@email.com"
            });
            await _dbConnection.InsertAsync(new Estudiante
            {
                NombreCompleto = "María Gómez",
                Legajo = "1002",
                Dni = "40333444",
                Comision = "Comisión A",
                Activo = true,
                Telefono = "+5491122222222",
                Email = "maria@email.com"
            });
            await _dbConnection.InsertAsync(new Estudiante
            {
                NombreCompleto = "Carlos Ruiz",
                Legajo = "1003",
                Dni = "40555666",
                Comision = "Comisión B",
                Activo = true,
                Telefono = "+5491133333333",
                Email = "carlos@email.com"
            });
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
                Comision = "Comisión B",
                NumeroClase = 1,
                Tema = "Introducción a Arduino y Sensores",
                Fecha = DateTime.Today.AddDays(-7)
            });
        }

        var cantidadTps = await _dbConnection.Table<TrabajoPractico>().CountAsync();
        if (cantidadTps == 0)
        {
            await _dbConnection.InsertAsync(new TrabajoPractico
            {
                Comision = "Comisión A",
                Nombre = "TP 1: Semáforo Inteligente",
                Descripcion = "Montaje con LEDs y pulsadores",
                FechaLimite = DateTime.Today.AddDays(5)
            });
        }
    }

    // =========================
    // PERSONAS
    // =========================

    public async Task<List<Persona>> GetPersonasAsync()
    {
        await InitAsync();
        return await _dbConnection!.Table<Persona>().ToListAsync();
    }

    public async Task<int> SavePersonaAsync(Persona persona)
    {
        await InitAsync();
        if (persona.Id != 0)
            return await _dbConnection!.UpdateAsync(persona);
        return await _dbConnection!.InsertAsync(persona);
    }

    public async Task<int> DeletePersonaAsync(Persona persona)
    {
        await InitAsync();
        return await _dbConnection!.DeleteAsync(persona);
    }

    // =========================
    // CLASES
    // =========================

    public async Task<List<Clase>> GetClasesAsync()
    {
        await InitAsync();
        return await _dbConnection!.Table<Clase>().ToListAsync();
    }

    public async Task<int> SaveClaseAsync(Clase clase)
    {
        await InitAsync();
        if (clase.Id != 0)
            return await _dbConnection!.UpdateAsync(clase);
        return await _dbConnection!.InsertAsync(clase);
    }

    public async Task<int> DeleteClaseAsync(Clase clase)
    {
        await InitAsync();
        return await _dbConnection!.DeleteAsync(clase);
    }

    // =========================
    // ESTUDIANTES
    // =========================

    public async Task<List<Estudiante>> GetEstudiantesAsync()
    {
        await InitAsync();
        return await _dbConnection!.Table<Estudiante>().ToListAsync();
    }

    public async Task<List<Estudiante>> GetEstudiantesPorComisionAsync(string nombreComision)
    {
        await InitAsync();

        if (string.IsNullOrWhiteSpace(nombreComision))
            return new List<Estudiante>();

        string nombreLimpio = nombreComision.Trim();

        // 1. Tabla Comisiones + relación EstudianteComision
        var comision = await _dbConnection!.Table<Comision>()
            .FirstOrDefaultAsync(c => c.Nombre != null &&
                                      c.Nombre.Trim().Equals(nombreLimpio, StringComparison.OrdinalIgnoreCase));

        if (comision != null)
        {
            var estComision = await GetEstudiantesPorComisionIdAsync(comision.Id);
            if (estComision != null && estComision.Any())
                return estComision;
        }

        // 2. Fallback: campo Comision del estudiante
        var estudiantes = await _dbConnection.Table<Estudiante>().ToListAsync();

        var filtrados = estudiantes
            .Where(e => e.Activo && !string.IsNullOrWhiteSpace(e.Comision) &&
                        (e.Comision.Trim().Equals(nombreLimpio, StringComparison.OrdinalIgnoreCase) ||
                         e.Comision.Trim().Contains(nombreLimpio, StringComparison.OrdinalIgnoreCase) ||
                         nombreLimpio.Contains(e.Comision.Trim(), StringComparison.OrdinalIgnoreCase)))
            .ToList();

        return filtrados;
    }

    public async Task<List<Estudiante>> GetEstudiantesPorComisionIdAsync(int comisionId)
    {
        await InitAsync();

        var relaciones = await _dbConnection!.Table<EstudianteComision>()
            .Where(ec => ec.ComisionId == comisionId)
            .ToListAsync();

        var estudianteIds = relaciones.Select(ec => ec.EstudianteId).ToList();

        if (!estudianteIds.Any())
            return new List<Estudiante>();

        return await _dbConnection.Table<Estudiante>()
            .Where(e => estudianteIds.Contains(e.Id) && e.Activo)
            .ToListAsync();
    }

    public async Task<List<Estudiante>> GetEstudiantesPorClaseIdAsync(int claseId)
    {
        await InitAsync();

        var inscripciones = await _dbConnection!.Table<Inscripcion>()
            .Where(i => i.ClaseId == claseId)
            .ToListAsync();

        var estudianteIds = inscripciones.Select(i => i.EstudianteId).ToList();

        if (!estudianteIds.Any())
            return new List<Estudiante>();

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
            return 0;

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
            return await _dbConnection!.UpdateAsync(estudiante);
        return await _dbConnection!.InsertAsync(estudiante);
    }

    public async Task<int> DeleteEstudianteAsync(Estudiante estudiante)
    {
        await InitAsync();
        return await _dbConnection!.DeleteAsync(estudiante);
    }

    // =========================
    // TRABAJOS PRÁCTICOS
    // =========================

    public async Task<List<TrabajoPractico>> GetTrabajosPracticosAsync()
    {
        await InitAsync();
        return await _dbConnection!.Table<TrabajoPractico>().ToListAsync();
    }

    public async Task<List<TrabajoPractico>> GetTrabajosPracticosPorComisionAsync(string comision)
    {
        await InitAsync();
        if (string.IsNullOrWhiteSpace(comision))
            return new List<TrabajoPractico>();

        string comisionLimpia = comision.Trim();
        var todosLosTps = await _dbConnection!.Table<TrabajoPractico>().ToListAsync();

        return todosLosTps
            .Where(tp => !string.IsNullOrWhiteSpace(tp.Comision) &&
                         (tp.Comision.Trim().Equals(comisionLimpia, StringComparison.OrdinalIgnoreCase) ||
                          tp.Comision.Trim().Contains(comisionLimpia, StringComparison.OrdinalIgnoreCase) ||
                          comisionLimpia.Contains(tp.Comision.Trim(), StringComparison.OrdinalIgnoreCase)))
            .ToList();
    }

    public async Task<int> SaveTrabajoPracticoAsync(TrabajoPractico tp)
    {
        await InitAsync();
        if (tp.Id != 0)
            return await _dbConnection!.UpdateAsync(tp);
        return await _dbConnection!.InsertAsync(tp);
    }

    public async Task<int> DeleteTrabajoPracticoAsync(TrabajoPractico tp)
    {
        await InitAsync();
        await _dbConnection!.ExecuteAsync("DELETE FROM EntregasTp WHERE TrabajoPracticoId = ?", tp.Id);
        return await _dbConnection!.DeleteAsync(tp);
    }

    public async Task<List<EntregaTp>> GetEntregasPorTpAsync(int trabajoPracticoId)
    {
        await InitAsync();
        return await _dbConnection!.Table<EntregaTp>()
            .Where(e => e.TrabajoPracticoId == trabajoPracticoId)
            .ToListAsync();
    }

    /// <summary>
    /// Método original (compatibilidad). Usa recálculo robusto.
    /// </summary>
    public async Task GuardarListaEntregasTpAsync(int trabajoPracticoId, List<EntregaTp> entregas)
    {
        await InitAsync();

        foreach (var entrega in entregas)
        {
            entrega.TrabajoPracticoId = trabajoPracticoId;

            if (entrega.Entregado && !entrega.FechaEntrega.HasValue)
                entrega.FechaEntrega = DateTime.Now;

            if (!entrega.Entregado)
                entrega.FechaEntrega = null;

            if (entrega.Id != 0)
                await _dbConnection!.UpdateAsync(entrega);
            else
                await _dbConnection!.InsertAsync(entrega);

            await RecalcularTpsPendientesEstudianteRobustoAsync(entrega.EstudianteId);
        }
    }

    /// <summary>
    /// Método NUEVO para el módulo de Registro de Entregas de TP.
    /// </summary>
    public async Task GuardarEntregasTrabajosPracticosAsync(int trabajoPracticoId, List<EntregaTp> entregas)
    {
        await InitAsync();

        foreach (var entrega in entregas)
        {
            entrega.TrabajoPracticoId = trabajoPracticoId;

            if (entrega.Entregado && !entrega.FechaEntrega.HasValue)
                entrega.FechaEntrega = DateTime.Now;

            if (!entrega.Entregado)
                entrega.FechaEntrega = null;

            if (entrega.Id != 0)
                await _dbConnection!.UpdateAsync(entrega);
            else
                await _dbConnection!.InsertAsync(entrega);

            await RecalcularTpsPendientesEstudianteRobustoAsync(entrega.EstudianteId);
        }
    }

    /// <summary>
    /// Cuenta como pendiente cada TP de la comisión del alumno que NO esté Entregado.
    /// Si no existe fila de entrega, también cuenta como pendiente.
    /// </summary>
    public async Task RecalcularTpsPendientesEstudianteRobustoAsync(int estudianteId)
    {
        await InitAsync();

        var estudiante = await _dbConnection!.Table<Estudiante>()
            .FirstOrDefaultAsync(e => e.Id == estudianteId);

        if (estudiante == null)
            return;

        string comision = (estudiante.Comision ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(comision))
        {
            estudiante.TpsNoEntregados = 0;
            await _dbConnection.UpdateAsync(estudiante);
            return;
        }

        var tpsComision = await _dbConnection.Table<TrabajoPractico>().ToListAsync();
        var idsTps = tpsComision
            .Where(tp => !string.IsNullOrWhiteSpace(tp.Comision) &&
                         (tp.Comision.Trim().Equals(comision, StringComparison.OrdinalIgnoreCase) ||
                          tp.Comision.Trim().Contains(comision, StringComparison.OrdinalIgnoreCase) ||
                          comision.Contains(tp.Comision.Trim(), StringComparison.OrdinalIgnoreCase)))
            .Select(tp => tp.Id)
            .ToList();

        if (!idsTps.Any())
        {
            estudiante.TpsNoEntregados = 0;
            await _dbConnection.UpdateAsync(estudiante);
            return;
        }

        var entregasAlumno = await _dbConnection.Table<EntregaTp>()
            .Where(e => e.EstudianteId == estudianteId)
            .ToListAsync();

        int pendientes = 0;
        foreach (var tpId in idsTps)
        {
            var reg = entregasAlumno.FirstOrDefault(e => e.TrabajoPracticoId == tpId);
            if (reg == null || !reg.Entregado)
                pendientes++;
        }

        estudiante.TpsNoEntregados = pendientes;
        await _dbConnection.UpdateAsync(estudiante);
    }

    // Compatibilidad con llamadas antiguas
    private async Task RecalcularTpsNoEntregadosEstudianteAsync(int estudianteId)
    {
        await RecalcularTpsPendientesEstudianteRobustoAsync(estudianteId);
    }

    // =========================
    // ASISTENCIAS
    // =========================

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
            if (ast.Fecha == default)
                ast.Fecha = DateTime.Today;

            if (ast.Id != 0)
                await _dbConnection!.UpdateAsync(ast);
            else
                await _dbConnection!.InsertAsync(ast);

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

    public async Task<bool> InscribirEstudiantePorDniAsync(string dni, int claseId)
    {
        await InitAsync();

        string dniLimpio = dni?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(dniLimpio))
            return false;

        var estudiante = await _dbConnection!.Table<Estudiante>()
            .FirstOrDefaultAsync(e => e.Dni == dniLimpio);

        if (estudiante == null)
            return false;

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

    // =========================
    // REPORTES
    // =========================

    public async Task<List<ReporteAsistenciaItem>> ObtenerReporteAsistenciasAsync(
        string? dniFiltro, string? comisionFiltro, DateTime? fechaInicio, DateTime? fechaFin)
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
                             FechaClase = (a.Fecha != default(DateTime) && a.Fecha.Year > 2000) ? a.Fecha : c.Fecha,
                             Presente = a.Presente,
                             Justificada = a.Justificada
                         };

        var query = listaCruda.AsQueryable();

        if (!string.IsNullOrWhiteSpace(dniFiltro))
        {
            string dniTrim = dniFiltro.Trim();
            query = query.Where(x => !string.IsNullOrEmpty(x.Dni) &&
                                     x.Dni.Contains(dniTrim, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(comisionFiltro) &&
            comisionFiltro != "Todas las comisiones" &&
            comisionFiltro != "General")
        {
            query = query.Where(x => !string.IsNullOrEmpty(x.Comision) &&
                                     x.Comision.Trim().Equals(comisionFiltro.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        if (fechaInicio.HasValue)
            query = query.Where(x => x.FechaClase.Date >= fechaInicio.Value.Date);

        if (fechaFin.HasValue)
            query = query.Where(x => x.FechaClase.Date <= fechaFin.Value.Date);

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
            query = query.Where(x => !string.IsNullOrEmpty(x.Dni) &&
                                     x.Dni.Contains(dniTrim, StringComparison.OrdinalIgnoreCase));
        }

        if (trabajoPracticoIdFiltro.HasValue && trabajoPracticoIdFiltro.Value > 0)
            query = query.Where(x => x.TrabajoPracticoId == trabajoPracticoIdFiltro.Value);

        return query.OrderBy(x => x.NombreEstudiante).ThenBy(x => x.NombreTp).ToList();
    }

    // =========================
    // COMISIONES / CLASES
    // =========================

    public async Task<List<Comision>> GetComisionesAsync()
    {
        await InitAsync();
        return await _dbConnection!.Table<Comision>().ToListAsync();
    }

    public async Task<int> GuardarUmbralesComisionAsync(Comision comision)
    {
        await InitAsync();
        if (comision.Id != 0)
            return await _dbConnection!.UpdateAsync(comision);
        return await _dbConnection!.InsertAsync(comision);
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
                await _dbConnection.InsertAsync(new Comision
                {
                    Nombre = nombreLimpio,
                    InasistenciasBajo = 2,
                    InasistenciasMedio = 4,
                    TpsBajo = 1,
                    TpsMedio = 2
                });
            }
        }

        if (clase.Id != 0)
            return await _dbConnection!.UpdateAsync(clase);
        return await _dbConnection!.InsertAsync(clase);
    }

    // =========================
    // INTERVENCIONES / CONTACTOS
    // =========================

    public async Task<List<IntervencionContacto>> GetIntervencionesPorEstudianteAsync(int estudianteId)
    {
        await InitAsync();
        return await _dbConnection!.Table<IntervencionContacto>()
            .Where(i => i.EstudianteId == estudianteId)
            .OrderByDescending(i => i.Fecha)
            .ToListAsync();
    }

    public async Task<int> GuardarIntervencionAsync(IntervencionContacto intervencion)
    {
        await InitAsync();
        return await _dbConnection!.InsertAsync(intervencion);
    }

    public async Task<IntervencionContacto?> GetUltimoContactoEstudianteAsync(int estudianteId)
    {
        await InitAsync();
        return await _dbConnection!.Table<IntervencionContacto>()
            .Where(i => i.EstudianteId == estudianteId)
            .OrderByDescending(i => i.Fecha)
            .FirstOrDefaultAsync();
    }

    public async Task<int> InscribirEstudianteEnComisionAsync(int estudianteId, int comisionId)
    {
        await InitAsync();

        var existente = await _dbConnection!.Table<EstudianteComision>()
            .FirstOrDefaultAsync(ec => ec.EstudianteId == estudianteId && ec.ComisionId == comisionId);

        if (existente != null)
            return 0;

        return await _dbConnection.InsertAsync(new EstudianteComision
        {
            EstudianteId = estudianteId,
            ComisionId = comisionId
        });
    }

    public async Task<List<ReporteContactoItemModel>> GetReporteContactosDetalladoAsync(int comisionId)
    {
        await InitAsync();

        var intervenciones = await _dbConnection!.Table<IntervencionContacto>().ToListAsync();
        var estudiantes = await _dbConnection.Table<Estudiante>().ToListAsync();
        var comisiones = await _dbConnection.Table<Comision>().ToListAsync();

        string? nombreComisionFiltro = null;
        if (comisionId > 0)
        {
            var com = comisiones.FirstOrDefault(c => c.Id == comisionId);
            nombreComisionFiltro = com?.Nombre?.Trim();
        }

        var query = from contacto in intervenciones
                    join est in estudiantes on contacto.EstudianteId equals est.Id
                    where comisionId == 0 ||
                          (est.Comision != null &&
                           est.Comision.Trim().Equals(nombreComisionFiltro, StringComparison.OrdinalIgnoreCase))
                    orderby contacto.Fecha descending
                    select new ReporteContactoItemModel
                    {
                        NombreEstudiante = est.NombreCompleto ?? string.Empty,
                        Legajo = est.Legajo ?? string.Empty,
                        ComisionNombre = est.Comision ?? string.Empty,
                        FechaContacto = contacto.Fecha,
                        MedioContacto = contacto.Medio ?? string.Empty,
                        Motivo = "Intervención Registrada",
                        ReporteSituacion = string.IsNullOrWhiteSpace(contacto.Resultado)
                            ? "Sin observaciones"
                            : contacto.Resultado,
                        AsistenciasAlMomento = est.Inasistencias,
                        TpsNoEntregadosAlMomento = est.TpsNoEntregados,
                        Mensaje = contacto.Resultado
                    };

        return query.ToList();
    }

    public async Task<List<IntervencionContacto>> GetIntervencionesAsync()
    {
        await InitAsync();
        return await _dbConnection!.Table<IntervencionContacto>().ToListAsync();
    }

    // =========================
    // IMPORTACIÓN MASIVA
    // =========================

    public async Task<int> ImportarEstructuraCompletaDesdeTxtAsync(Stream fileStream)
    {
        await InitAsync();
        int registrosProcesados = 0;

        using (var reader = new StreamReader(fileStream))
        {
            string seccionActual = string.Empty;

            await _dbConnection!.RunInTransactionAsync((SQLiteConnection db) =>
            {
                while (!reader.EndOfStream)
                {
                    string? linea = reader.ReadLine()?.Trim();
                    if (string.IsNullOrWhiteSpace(linea))
                        continue;

                    string lineaUpper = linea.ToUpperInvariant();

                    // Detección de sección
                    if ((linea.StartsWith("[") && linea.EndsWith("]")) || lineaUpper.Contains("---"))
                    {
                        if (lineaUpper.Contains("COMISIONES")) seccionActual = "comisiones";
                        else if (lineaUpper.Contains("ESTUDIANTES")) seccionActual = "estudiantes";
                        else if (lineaUpper.Contains("CLASES")) seccionActual = "clases";
                        else if (lineaUpper.Contains("TRABAJOS_PRACTICOS") || lineaUpper.Contains("TPS")) seccionActual = "trabajospracticos";
                        else if (lineaUpper.Contains("ASISTENCIAS")) seccionActual = "asistencias";
                        else if (lineaUpper.Contains("ENTREGAS_TP")) seccionActual = "entregasp";
                        continue;
                    }

                    if (linea.StartsWith("#"))
                        continue;

                    var datos = linea.Split(';');
                    if (datos.Length < 2)
                        continue;

                    switch (seccionActual)
                    {
                        case "comisiones":
                            if (datos.Length >= 5)
                            {
                                var com = new Comision
                                {
                                    Nombre = datos[0].Trim(),
                                    InasistenciasBajo = int.Parse(datos[1]),
                                    InasistenciasMedio = int.Parse(datos[2]),
                                    TpsBajo = int.Parse(datos[3]),
                                    TpsMedio = int.Parse(datos[4])
                                };
                                db.Insert(com);
                                registrosProcesados++;
                            }
                            break;

                        case "estudiantes":
                            if (datos.Length >= 8)
                            {
                                var est = new Estudiante
                                {
                                    Legajo = datos[0].Trim(),
                                    Dni = datos[1].Trim(),
                                    NombreCompleto = datos[2].Trim(),
                                    Email = datos[3].Trim(),
                                    Telefono = datos[4].Trim(),
                                    Comision = datos[5].Trim(),
                                    Activo = true,
                                    Inasistencias = int.Parse(datos[6]),
                                    TpsNoEntregados = int.Parse(datos[7])
                                };
                                db.Insert(est);
                                registrosProcesados++;
                            }
                            break;

                        case "clases":
                            if (datos.Length >= 5)
                            {
                                var cls = new Clase
                                {
                                    NombreMateria = datos[0].Trim(),
                                    Comision = datos[1].Trim(),
                                    Fecha = DateTime.TryParse(datos[2], out var fClase) ? fClase : DateTime.Today,
                                    NumeroClase = int.Parse(datos[3]),
                                    Tema = datos[4].Trim()
                                };
                                db.Insert(cls);
                                registrosProcesados++;
                            }
                            break;

                        case "trabajospracticos":
                            // Nombre;Descripcion;FechaLimite[;Comision]
                            if (datos.Length >= 3)
                            {
                                var tp = new TrabajoPractico
                                {
                                    Nombre = datos[0].Trim(),
                                    Descripcion = datos[1].Trim(),
                                    FechaLimite = DateTime.TryParse(datos[2], out var fTp) ? fTp : DateTime.Today.AddDays(5),
                                    Comision = datos.Length >= 4 ? datos[3].Trim() : string.Empty
                                };
                                db.Insert(tp);
                                registrosProcesados++;
                            }
                            break;

                        case "asistencias":
                            if (datos.Length >= 5)
                            {
                                var ast = new Asistencia
                                {
                                    ClaseId = int.Parse(datos[0]),
                                    EstudianteId = int.Parse(datos[1]),
                                    Presente = datos[2].Trim() == "1",
                                    Justificada = datos[3].Trim() == "1",
                                    Fecha = DateTime.TryParse(datos[4], out var fAst) ? fAst : DateTime.Today
                                };
                                db.Insert(ast);
                                registrosProcesados++;
                            }
                            break;

                        case "entregasp":
                            if (datos.Length >= 4)
                            {
                                bool entregado = datos[2].Trim() == "1";
                                var ent = new EntregaTp
                                {
                                    TrabajoPracticoId = int.Parse(datos[0]),
                                    EstudianteId = int.Parse(datos[1]),
                                    Entregado = entregado,
                                    FechaEntrega = (entregado && DateTime.TryParse(datos[3], out var fEnt)) ? fEnt : null
                                };
                                db.Insert(ent);
                                registrosProcesados++;
                            }
                            break;
                    }
                }
            });
        }

        return registrosProcesados;
    }


    public async Task<List<Estudiante>> GetEstudiantesParaTpExclusivoAsync(int claseId, string comisionClase)
    {
        await InitAsync();

        List<Estudiante> estudiantes = new();

        string nombreComisionExtraido = comisionClase?.Trim() ?? string.Empty;
        if (nombreComisionExtraido.Contains("[") && nombreComisionExtraido.Contains("]"))
        {
            int inicio = nombreComisionExtraido.IndexOf('[') + 1;
            int fin = nombreComisionExtraido.IndexOf(']');
            if (fin > inicio)
            {
                nombreComisionExtraido = nombreComisionExtraido.Substring(inicio, fin - inicio).Trim();
            }
        }

        System.Diagnostics.Debug.WriteLine($"[TP DEBUG] Buscando alumnos para ClaseId: {claseId}, Comisión extraída: '{nombreComisionExtraido}'");

        // 1. Buscar por la tabla de Comisiones y EstudianteComision
        var comisionObj = await _dbConnection!.Table<Comision>()
            .FirstOrDefaultAsync(c => c.Nombre != null && c.Nombre.Trim().Equals(nombreComisionExtraido, StringComparison.OrdinalIgnoreCase));

        if (comisionObj != null)
        {
            var relaciones = await _dbConnection.Table<EstudianteComision>()
                .Where(ec => ec.ComisionId == comisionObj.Id)
                .ToListAsync();

            var idsEstudiantes = relaciones.Select(ec => ec.EstudianteId).ToList();
            if (idsEstudiantes.Any())
            {
                estudiantes = await _dbConnection.Table<Estudiante>()
                    .Where(e => idsEstudiantes.Contains(e.Id) && e.Activo)
                    .ToListAsync();
            }
        }

        // 2. Si no hay, buscar por el campo texto 'Comision' en Estudiantes
        if (!estudiantes.Any() && !string.IsNullOrWhiteSpace(nombreComisionExtraido))
        {
            var todosEstudiantes = await _dbConnection.Table<Estudiante>()
                .Where(e => e.Activo && !string.IsNullOrWhiteSpace(e.Comision))
                .ToListAsync();

            estudiantes = todosEstudiantes
                .Where(e => e.Comision!.Trim().Equals(nombreComisionExtraido, StringComparison.OrdinalIgnoreCase) ||
                            e.Comision!.Trim().Contains(nombreComisionExtraido, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        // 3. Fallback absoluto: Si aun así está vacío, traemos TODOS los alumnos activos para asegurarnos de que la interfaz pinte la lista y puedas trabajar
        if (!estudiantes.Any())
        {
            System.Diagnostics.Debug.WriteLine("[TP DEBUG] No se encontraron por comisión. Usando respaldo general de alumnos activos.");
            estudiantes = await _dbConnection.Table<Estudiante>()
                .Where(e => e.Activo)
                .ToListAsync();
        }

        System.Diagnostics.Debug.WriteLine($"[TP DEBUG] Total alumnos encontrados para la vista: {estudiantes.Count}");
        return estudiantes;
    }
}