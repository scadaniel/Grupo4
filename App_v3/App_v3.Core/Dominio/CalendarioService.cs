namespace App_v3.Dominio;

/// <summary>H2: calendario de clases y de trabajos prácticos de una comisión.</summary>
public class CalendarioService(IRepositorio repositorio)
{
    // CA 2.1 y CA 2.4
    public async Task<ClaseDictada> AgregarClaseAsync(int comisionId, DateTime? fecha, string? tema)
    {
        var faltantes = CamposFaltantes((fecha is null, "Fecha"), (string.IsNullOrWhiteSpace(tema), "Tema"));
        if (faltantes.Count > 0) throw FaltanDatos(faltantes);

        await ExigirComisionAsync(comisionId);
        var dia = fecha!.Value.Date;
        var existentes = await ListarClasesAsync(comisionId);
        if (existentes.Any(c => c.Fecha.Date == dia))
            throw new ReglaDeNegocioException($"Ya hay una clase cargada el {dia:dd/MM/yyyy} en esta comisión.");

        var clase = new ClaseDictada
        {
            ComisionId = comisionId,
            Fecha = dia,
            Numero = existentes.Count == 0 ? 1 : existentes.Max(c => c.Numero) + 1,
            Tema = tema!.Trim()
        };
        await repositorio.GuardarAsync(clase);
        return clase;
    }

    // CA 2.2 y CA 2.4
    public async Task<TrabajoPractico> AgregarTrabajoAsync(int comisionId, string? nombre, DateTime? fechaLimite)
    {
        var faltantes = CamposFaltantes((string.IsNullOrWhiteSpace(nombre), "Nombre"), (fechaLimite is null, "Fecha límite"));
        if (faltantes.Count > 0) throw FaltanDatos(faltantes);

        await ExigirComisionAsync(comisionId);
        var nombreLimpio = nombre!.Trim();
        var existentes = await ListarTrabajosAsync(comisionId);
        if (existentes.Any(t => string.Equals(t.Nombre, nombreLimpio, StringComparison.OrdinalIgnoreCase)))
            throw new ReglaDeNegocioException($"Ya hay un trabajo práctico llamado {nombreLimpio} en esta comisión.");

        var trabajo = new TrabajoPractico { ComisionId = comisionId, Nombre = nombreLimpio, FechaLimite = fechaLimite!.Value.Date };
        await repositorio.GuardarAsync(trabajo);
        return trabajo;
    }

    public async Task<List<ClaseDictada>> ListarClasesAsync(int comisionId) =>
        (await repositorio.ListarAsync<ClaseDictada>())
            .Where(c => c.ComisionId == comisionId)
            .OrderBy(c => c.Fecha).ThenBy(c => c.Numero)
            .ToList();

    public async Task<List<TrabajoPractico>> ListarTrabajosAsync(int comisionId) =>
        (await repositorio.ListarAsync<TrabajoPractico>())
            .Where(t => t.ComisionId == comisionId)
            .OrderBy(t => t.FechaLimite).ThenBy(t => t.Nombre)
            .ToList();

    // CA 2.3: clases y trabajos juntos, en orden cronológico.
    public async Task<List<EventoCalendario>> ListarEventosAsync(int comisionId)
    {
        var clases = (await ListarClasesAsync(comisionId))
            .Select(c => new EventoCalendario(TipoEvento.Clase, c.Id, c.Fecha, $"Clase {c.Numero}: {c.Tema}"));
        var trabajos = (await ListarTrabajosAsync(comisionId))
            .Select(t => new EventoCalendario(TipoEvento.TrabajoPractico, t.Id, t.FechaLimite, $"Entrega: {t.Nombre}"));

        return clases.Concat(trabajos).OrderBy(e => e.Fecha).ThenBy(e => e.Tipo).ToList();
    }

    private async Task ExigirComisionAsync(int comisionId)
    {
        var comisiones = await repositorio.ListarAsync<Comision>();
        if (!comisiones.Any(c => c.Id == comisionId))
            throw new ReglaDeNegocioException("Elija una comisión antes de cargar el calendario.");
    }

    private static List<string> CamposFaltantes(params (bool Falta, string Campo)[] campos) =>
        campos.Where(c => c.Falta).Select(c => c.Campo).ToList();

    private static ReglaDeNegocioException FaltanDatos(List<string> faltantes) =>
        new("Complete los campos obligatorios: " + string.Join(", ", faltantes) + ".");
}
