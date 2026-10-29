namespace App_v3.Dominio;

/// <summary>Un estudiante tal como figura en la nómina de una comisión.</summary>
public record InscriptoEnComision(Estudiante Estudiante, bool Activo, DateTime? FechaBaja);

/// <summary>
/// H1 y H13: inscripción de estudiantes a comisiones, baja lógica y reactivación.
/// Estar activo o inactivo depende de la comisión: la baja en una no afecta a las otras.
/// </summary>
public class InscripcionService(IRepositorio repositorio)
{
    public async Task<Inscripcion> InscribirAsync(int estudianteId, int comisionId)
    {
        await ExigirExistenciaAsync(estudianteId, comisionId);
        var existente = await BuscarAsync(estudianteId, comisionId);

        if (existente is null)
            return await CrearAsync(estudianteId, comisionId);
        if (existente.Activa)
            throw new ReglaDeNegocioException("El estudiante ya está inscripto en esta comisión.");

        return await CambiarEstadoAsync(existente, activa: true, fechaBaja: null);
    }

    // CA 1.4 y CA 13.3: no se borra nada, el historial sigue asociado al estudiante.
    public async Task DarDeBajaAsync(int estudianteId, int comisionId, DateTime fecha)
    {
        var inscripcion = await ObtenerOFallarAsync(estudianteId, comisionId);
        if (!inscripcion.Activa) return;

        await CambiarEstadoAsync(inscripcion, activa: false, fechaBaja: fecha);
    }

    public async Task ReactivarAsync(int estudianteId, int comisionId)
    {
        var inscripcion = await ObtenerOFallarAsync(estudianteId, comisionId);
        if (inscripcion.Activa) return;

        await CambiarEstadoAsync(inscripcion, activa: true, fechaBaja: null);
    }

    // CA 1.3: todos los estudiantes de la comisión, con su estado.
    public async Task<List<InscriptoEnComision>> ListarNominaAsync(int comisionId)
    {
        var inscripciones = await repositorio.ListarAsync<Inscripcion>();
        var estudiantes = (await repositorio.ListarAsync<Estudiante>()).ToDictionary(e => e.Id);

        return inscripciones
            .Where(i => i.ComisionId == comisionId && estudiantes.ContainsKey(i.EstudianteId))
            .Select(i => new InscriptoEnComision(estudiantes[i.EstudianteId], i.Activa, i.FechaBaja))
            .OrderBy(x => x.Estudiante.Apellido, StringComparer.CurrentCultureIgnoreCase)
            .ThenBy(x => x.Estudiante.Nombre, StringComparer.CurrentCultureIgnoreCase)
            .ToList();
    }

    // CA 6.4: los inactivos no entran al panel de seguimiento.
    public async Task<List<Estudiante>> ListarActivosAsync(int comisionId) =>
        (await ListarNominaAsync(comisionId)).Where(x => x.Activo).Select(x => x.Estudiante).ToList();

    // CA 13.1
    public async Task<List<Estudiante>> ListarInactivosAsync(int comisionId) =>
        (await ListarNominaAsync(comisionId)).Where(x => !x.Activo).Select(x => x.Estudiante).ToList();

    private async Task<Inscripcion> CrearAsync(int estudianteId, int comisionId)
    {
        var nueva = new Inscripcion { EstudianteId = estudianteId, ComisionId = comisionId };
        await repositorio.GuardarAsync(nueva);
        return nueva;
    }

    private async Task<Inscripcion> CambiarEstadoAsync(Inscripcion inscripcion, bool activa, DateTime? fechaBaja)
    {
        inscripcion.Activa = activa;
        inscripcion.FechaBaja = fechaBaja;
        await repositorio.GuardarAsync(inscripcion);
        return inscripcion;
    }

    private async Task<Inscripcion?> BuscarAsync(int estudianteId, int comisionId) =>
        (await repositorio.ListarAsync<Inscripcion>())
            .FirstOrDefault(i => i.EstudianteId == estudianteId && i.ComisionId == comisionId);

    private async Task<Inscripcion> ObtenerOFallarAsync(int estudianteId, int comisionId) =>
        await BuscarAsync(estudianteId, comisionId)
            ?? throw new ReglaDeNegocioException("El estudiante no está inscripto en esta comisión.");

    private async Task ExigirExistenciaAsync(int estudianteId, int comisionId)
    {
        var estudiantes = await repositorio.ListarAsync<Estudiante>();
        if (!estudiantes.Any(e => e.Id == estudianteId))
            throw new ReglaDeNegocioException("El estudiante no existe.");

        var comisiones = await repositorio.ListarAsync<Comision>();
        if (!comisiones.Any(c => c.Id == comisionId))
            throw new ReglaDeNegocioException("La comisión no existe.");
    }
}
