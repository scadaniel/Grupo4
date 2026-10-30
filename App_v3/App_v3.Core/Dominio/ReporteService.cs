namespace App_v3.Dominio;

/// <summary>Un renglón del reporte de asistencias: un estudiante en una clase.</summary>
public record FilaReporteAsistencia(Estudiante Estudiante, ClaseDictada Clase, EstadoAsistencia Estado);

/// <summary>Un renglón del reporte de entregas: un estudiante activo frente a un trabajo práctico.</summary>
public record FilaReporteEntrega(Estudiante Estudiante, TrabajoPractico Trabajo, EstadoEntrega Estado);

/// <summary>Reportes de asistencias y de entregas de una comisión, con filtros.</summary>
public class ReporteService(IRepositorio repositorio, InscripcionService inscripciones, TimeProvider reloj)
{
    /// <summary>Asistencias registradas en la comisión. Incluye a los estudiantes dados de baja (H13).</summary>
    public async Task<List<FilaReporteAsistencia>> AsistenciasAsync(
        int comisionId, string? dni = null, DateTime? desde = null, DateTime? hasta = null)
    {
        var nomina = await inscripciones.ListarNominaAsync(comisionId);
        var estudiantes = FiltrarPorDni(nomina.Select(n => n.Estudiante), dni).ToDictionary(e => e.Id);
        var clases = (await repositorio.ListarAsync<ClaseDictada>())
            .Where(c => c.ComisionId == comisionId && EstaEnRango(c.Fecha, desde, hasta))
            .ToDictionary(c => c.Id);

        return (await repositorio.ListarAsync<Asistencia>())
            .Where(a => estudiantes.ContainsKey(a.EstudianteId) && clases.ContainsKey(a.ClaseDictadaId))
            .Select(a => new FilaReporteAsistencia(estudiantes[a.EstudianteId], clases[a.ClaseDictadaId], a.Estado))
            .OrderByDescending(f => f.Clase.Fecha)
            .ThenBy(f => f.Estudiante.Apellido, StringComparer.CurrentCultureIgnoreCase)
            .ThenBy(f => f.Estudiante.Nombre, StringComparer.CurrentCultureIgnoreCase)
            .ToList();
    }

    /// <summary>Cada estudiante activo frente a cada trabajo práctico de la comisión.</summary>
    public async Task<List<FilaReporteEntrega>> EntregasAsync(int comisionId, string? dni = null, int? trabajoId = null)
    {
        var activos = FiltrarPorDni(await inscripciones.ListarActivosAsync(comisionId), dni).ToList();
        var trabajos = (await repositorio.ListarAsync<TrabajoPractico>())
            .Where(t => t.ComisionId == comisionId && (trabajoId is null || t.Id == trabajoId))
            .ToList();
        var entregas = await repositorio.ListarAsync<Entrega>();
        var hoy = reloj.GetLocalNow().DateTime;

        return activos
            .SelectMany(e => trabajos.Select(t => new FilaReporteEntrega(e, t, EstadoDe(entregas, e, t, hoy))))
            .OrderBy(f => f.Estudiante.Apellido, StringComparer.CurrentCultureIgnoreCase)
            .ThenBy(f => f.Estudiante.Nombre, StringComparer.CurrentCultureIgnoreCase)
            .ThenBy(f => f.Trabajo.FechaLimite)
            .ToList();
    }

    private static EstadoEntrega EstadoDe(List<Entrega> entregas, Estudiante estudiante, TrabajoPractico trabajo, DateTime hoy)
    {
        var entrega = entregas.FirstOrDefault(x => x.TrabajoPracticoId == trabajo.Id && x.EstudianteId == estudiante.Id);
        return ReglasDeEntrega.EstadoDe(trabajo, entrega, hoy);
    }

    private static IEnumerable<Estudiante> FiltrarPorDni(IEnumerable<Estudiante> estudiantes, string? dni)
    {
        if (string.IsNullOrWhiteSpace(dni)) return estudiantes;

        var buscado = ValidacionEstudiante.LimpiarDni(dni);
        return estudiantes.Where(e => e.Dni.Contains(buscado, StringComparison.Ordinal));
    }

    private static bool EstaEnRango(DateTime fecha, DateTime? desde, DateTime? hasta) =>
        (desde is null || fecha.Date >= desde.Value.Date) && (hasta is null || fecha.Date <= hasta.Value.Date);
}
