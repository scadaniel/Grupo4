namespace App_v3.Dominio;

/// <summary>
/// H6, H8, H9, H11, H12 y H13: calcula el riesgo de cada estudiante y arma el panel y la ficha.
/// Todo se calcula al consultar, así que registrar o corregir una asistencia, una entrega
/// o los umbrales se refleja solo (CA 6.3, CA 8.4).
/// </summary>
public class SeguimientoService(
    IRepositorio repositorio,
    InscripcionService inscripciones,
    UmbralesService umbrales,
    TimeProvider reloj)
{
    // Un contacto de hace 7 días o menos cuenta como "de esta semana" (CA 11.2).
    private const int DiasParaSerReciente = 7;

    /// <summary>
    /// CA 8.1: estudiantes activos de la comisión, de mayor a menor riesgo.
    /// Con soloNoContactadosEstaSemana (CA 11.2) quedan los de riesgo Medio o Alto sin contacto reciente.
    /// </summary>
    public async Task<List<SituacionEstudiante>> PanelAsync(int comisionId, bool soloNoContactadosEstaSemana = false)
    {
        var datos = await CargarDatosAsync(comisionId);
        var activos = await inscripciones.ListarActivosAsync(comisionId);

        var filas = activos.Select(e => SituacionDe(datos, e));
        if (soloNoContactadosEstaSemana)
            filas = filas.Where(f => EstaSinContactoReciente(f, datos.Hoy));

        return filas
            .OrderByDescending(f => f.Nivel)
            .ThenBy(f => f.Estudiante.Apellido, StringComparer.CurrentCultureIgnoreCase)
            .ThenBy(f => f.Estudiante.Nombre, StringComparer.CurrentCultureIgnoreCase)
            .ToList();
    }

    /// <summary>CA 9.1 a 9.3, CA 12.1 a 12.4 y CA 13.2: ficha de un estudiante, activo o inactivo.</summary>
    public async Task<FichaEstudiante> ObtenerFichaAsync(int estudianteId, int comisionId)
    {
        var nomina = await inscripciones.ListarNominaAsync(comisionId);
        var inscripto = nomina.FirstOrDefault(n => n.Estudiante.Id == estudianteId)
            ?? throw new ReglaDeNegocioException("El estudiante no está inscripto en esta comisión.");

        var datos = await CargarDatosAsync(comisionId);
        var situacion = SituacionDe(datos, inscripto.Estudiante);

        return new FichaEstudiante(
            inscripto.Estudiante,
            inscripto.Activo,
            inscripto.FechaBaja,
            inscripto.Activo ? situacion.Nivel : null,
            situacion.Motivo,
            AsistenciasDe(datos, estudianteId),
            EntregasDe(datos, estudianteId),
            datos.Contactos.Where(c => c.EstudianteId == estudianteId).OrderByDescending(c => c.Fecha).ToList());
    }

    // CA 6.1 y CA 6.2: las dos señales, con los umbrales configurados en H7.
    private static SituacionEstudiante SituacionDe(DatosDeComision datos, Estudiante estudiante)
    {
        var inasistencias = datos.Asistencias.Count(a => a.EstudianteId == estudiante.Id && a.CuentaComoInasistencia);
        var tpsSinEntregar = datos.Trabajos.Count(t =>
            ReglasDeEntrega.EstadoDe(t, EntregaDe(datos, t, estudiante.Id), datos.Hoy) == EstadoEntrega.NoEntregado);

        var ultimoContacto = datos.Contactos
            .Where(c => c.EstudianteId == estudiante.Id)
            .Select(c => (DateTime?)c.Fecha)
            .Max();

        return new SituacionEstudiante(
            estudiante,
            datos.Umbrales.CalcularNivel(inasistencias, tpsSinEntregar),
            inasistencias,
            tpsSinEntregar,
            ultimoContacto);
    }

    private static bool EstaSinContactoReciente(SituacionEstudiante fila, DateTime hoy) =>
        fila.Nivel != NivelRiesgo.Bajo &&
        (fila.UltimoContacto is null || fila.UltimoContacto.Value.Date < hoy.Date.AddDays(-DiasParaSerReciente));

    private static List<RegistroDeAsistencia> AsistenciasDe(DatosDeComision datos, int estudianteId) =>
        datos.Clases
            .Select(c => (Clase: c, Registro: datos.Asistencias.FirstOrDefault(a => a.ClaseDictadaId == c.Id && a.EstudianteId == estudianteId)))
            .Where(x => x.Registro is not null)
            .Select(x => new RegistroDeAsistencia(x.Clase, x.Registro!.Estado))
            .ToList();

    private static List<RegistroDeEntrega> EntregasDe(DatosDeComision datos, int estudianteId) =>
        datos.Trabajos
            .Select(t => new RegistroDeEntrega(t, ReglasDeEntrega.EstadoDe(t, EntregaDe(datos, t, estudianteId), datos.Hoy)))
            .ToList();

    private static Entrega? EntregaDe(DatosDeComision datos, TrabajoPractico trabajo, int estudianteId) =>
        datos.Entregas.FirstOrDefault(e => e.TrabajoPracticoId == trabajo.Id && e.EstudianteId == estudianteId);

    // Se leen una sola vez los datos de la comisión y después se calcula en memoria.
    private async Task<DatosDeComision> CargarDatosAsync(int comisionId)
    {
        var clases = (await repositorio.ListarAsync<ClaseDictada>()).Where(c => c.ComisionId == comisionId).OrderBy(c => c.Fecha).ToList();
        var trabajos = (await repositorio.ListarAsync<TrabajoPractico>()).Where(t => t.ComisionId == comisionId).OrderBy(t => t.FechaLimite).ToList();
        var idsClases = clases.Select(c => c.Id).ToHashSet();
        var idsTrabajos = trabajos.Select(t => t.Id).ToHashSet();

        return new DatosDeComision(
            await umbrales.ObtenerAsync(),
            reloj.GetLocalNow().DateTime,
            clases,
            trabajos,
            (await repositorio.ListarAsync<Asistencia>()).Where(a => idsClases.Contains(a.ClaseDictadaId)).ToList(),
            (await repositorio.ListarAsync<Entrega>()).Where(e => idsTrabajos.Contains(e.TrabajoPracticoId)).ToList(),
            (await repositorio.ListarAsync<Contacto>()).Where(c => c.ComisionId == comisionId).ToList());
    }

    private sealed record DatosDeComision(
        UmbralesRiesgo Umbrales,
        DateTime Hoy,
        List<ClaseDictada> Clases,
        List<TrabajoPractico> Trabajos,
        List<Asistencia> Asistencias,
        List<Entrega> Entregas,
        List<Contacto> Contactos);
}
