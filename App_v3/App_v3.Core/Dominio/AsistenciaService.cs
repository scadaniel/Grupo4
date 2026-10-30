namespace App_v3.Dominio;

/// <summary>H3 y H4: tomar la asistencia de una clase y corregirla después.</summary>
public class AsistenciaService(IRepositorio repositorio, InscripcionService inscripciones)
{
    // CA 3.1, CA 3.4 y CA 4.1: todos presentes si no hay registro; los estados guardados si ya lo hay.
    public async Task<PlanillaAsistencia> ObtenerPlanillaAsync(int claseId)
    {
        var clase = await ObtenerClaseAsync(claseId);
        var activos = await inscripciones.ListarActivosAsync(clase.ComisionId);
        var guardadas = await ListarDeLaClaseAsync(claseId);

        var lineas = activos.Select(e => LineaDe(e, guardadas)).ToList();
        return new PlanillaAsistencia(clase, guardadas.Count > 0, lineas);
    }

    // CA 3.3: guarda de una vez a todos los estudiantes de la clase.
    public Task GuardarAsync(int claseId, IEnumerable<LineaAsistencia> lineas) =>
        PersistirAsync(claseId, lineas, esCorreccion: false);

    // CA 4.3: corrige una asistencia ya guardada. El riesgo no se guarda, así que se recalcula solo.
    public Task CorregirAsync(int claseId, IEnumerable<LineaAsistencia> lineas) =>
        PersistirAsync(claseId, lineas, esCorreccion: true);

    private async Task PersistirAsync(int claseId, IEnumerable<LineaAsistencia> lineas, bool esCorreccion)
    {
        var planilla = await ObtenerPlanillaAsync(claseId);
        var recibidas = lineas.ToList();
        ExigirEstadoValido(planilla, esCorreccion);
        ExigirEstudiantesActivos(planilla, recibidas);

        var guardadas = await ListarDeLaClaseAsync(claseId);
        foreach (var linea in recibidas)
            await repositorio.GuardarAsync(RegistroDe(claseId, linea, guardadas));
    }

    private static void ExigirEstadoValido(PlanillaAsistencia planilla, bool esCorreccion)
    {
        if (planilla.Lineas.Count == 0)
            throw new ReglaDeNegocioException("No hay estudiantes activos en la comisión.");

        // CA 3.4: ya existe un registro, el camino es corregirlo.
        if (planilla.YaRegistrada && !esCorreccion)
            throw new ReglaDeNegocioException(
                $"Ya existe un registro de asistencia para el {planilla.Clase.Fecha:dd/MM/yyyy}. Use Modificar asistencia.");

        if (!planilla.YaRegistrada && esCorreccion)
            throw new ReglaDeNegocioException("Todavía no hay asistencia guardada para esa clase.");
    }

    private static void ExigirEstudiantesActivos(PlanillaAsistencia planilla, List<LineaAsistencia> recibidas)
    {
        var permitidos = planilla.Lineas.Select(l => l.Estudiante.Id).ToHashSet();
        var ajeno = recibidas.FirstOrDefault(l => !permitidos.Contains(l.Estudiante.Id));

        if (ajeno is not null)
            throw new ReglaDeNegocioException(
                $"{ajeno.Estudiante.NombreCompleto} no está activo en la comisión de esta clase.");
    }

    private static LineaAsistencia LineaDe(Estudiante estudiante, List<Asistencia> guardadas)
    {
        var existente = guardadas.FirstOrDefault(a => a.EstudianteId == estudiante.Id);
        return existente is null
            ? new LineaAsistencia(estudiante)
            : new LineaAsistencia(estudiante, existente.Presente, existente.Justificada);
    }

    private static Asistencia RegistroDe(int claseId, LineaAsistencia linea, List<Asistencia> guardadas)
    {
        var registro = guardadas.FirstOrDefault(a => a.EstudianteId == linea.Estudiante.Id)
                       ?? new Asistencia { ClaseDictadaId = claseId, EstudianteId = linea.Estudiante.Id };

        registro.Presente = linea.Presente;
        registro.Justificada = !linea.Presente && linea.Justificada; // CA 3.5: solo se justifica una ausencia
        return registro;
    }

    private async Task<ClaseDictada> ObtenerClaseAsync(int claseId) =>
        (await repositorio.ListarAsync<ClaseDictada>()).FirstOrDefault(c => c.Id == claseId)
            ?? throw new ReglaDeNegocioException("Elija una clase del calendario.");

    private async Task<List<Asistencia>> ListarDeLaClaseAsync(int claseId) =>
        (await repositorio.ListarAsync<Asistencia>()).Where(a => a.ClaseDictadaId == claseId).ToList();
}
