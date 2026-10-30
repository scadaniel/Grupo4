namespace App_v3.Dominio;

/// <summary>H5: registrar quién entregó y quién no entregó cada trabajo práctico.</summary>
public class EntregaService(IRepositorio repositorio, InscripcionService inscripciones)
{
    // CA 5.3 y CA 5.4: estudiantes activos de la comisión, con lo que ya estaba guardado.
    public async Task<PlanillaEntregas> ObtenerPlanillaAsync(int trabajoId)
    {
        var trabajo = await ObtenerTrabajoAsync(trabajoId);
        var activos = await inscripciones.ListarActivosAsync(trabajo.ComisionId);
        var guardadas = await ListarDelTrabajoAsync(trabajoId);

        var lineas = activos
            .Select(e => new LineaEntrega(e, guardadas.FirstOrDefault(g => g.EstudianteId == e.Id)?.Entregado ?? false))
            .ToList();
        return new PlanillaEntregas(trabajo, guardadas.Count > 0, lineas);
    }

    // CA 5.1 y CA 5.2: guarda "Entregó" y "No entregó". Se puede volver a guardar para corregir.
    public async Task GuardarAsync(int trabajoId, IEnumerable<LineaEntrega> lineas)
    {
        var planilla = await ObtenerPlanillaAsync(trabajoId);
        var recibidas = lineas.ToList();
        ExigirEstudiantesActivos(planilla, recibidas);

        var guardadas = await ListarDelTrabajoAsync(trabajoId);
        foreach (var linea in recibidas)
        {
            var registro = guardadas.FirstOrDefault(g => g.EstudianteId == linea.Estudiante.Id)
                           ?? new Entrega { TrabajoPracticoId = trabajoId, EstudianteId = linea.Estudiante.Id };
            registro.Entregado = linea.Entregado;
            await repositorio.GuardarAsync(registro);
        }
    }

    private static void ExigirEstudiantesActivos(PlanillaEntregas planilla, List<LineaEntrega> recibidas)
    {
        if (planilla.Lineas.Count == 0)
            throw new ReglaDeNegocioException("No hay estudiantes activos en la comisión.");

        var permitidos = planilla.Lineas.Select(l => l.Estudiante.Id).ToHashSet();
        var ajeno = recibidas.FirstOrDefault(l => !permitidos.Contains(l.Estudiante.Id));
        if (ajeno is not null)
            throw new ReglaDeNegocioException(
                $"{ajeno.Estudiante.NombreCompleto} no está activo en la comisión de este trabajo.");
    }

    private async Task<TrabajoPractico> ObtenerTrabajoAsync(int trabajoId) =>
        (await repositorio.ListarAsync<TrabajoPractico>()).FirstOrDefault(t => t.Id == trabajoId)
            ?? throw new ReglaDeNegocioException("Elija un trabajo práctico del calendario.");

    private async Task<List<Entrega>> ListarDelTrabajoAsync(int trabajoId) =>
        (await repositorio.ListarAsync<Entrega>()).Where(e => e.TrabajoPracticoId == trabajoId).ToList();
}
