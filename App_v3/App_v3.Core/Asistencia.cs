namespace App_v3.Core;

public class AsistenciaYaRegistradaException(DateOnly fecha)
    : InvalidOperationException($"Ya existe un registro de asistencia para el {fecha:dd/MM/yyyy}.");

public class ListaAsistencia
{
    private readonly Dictionary<string, bool> _estados;

    public DateOnly Fecha { get; }
    public IReadOnlyDictionary<string, bool> Estados => _estados;

    public ListaAsistencia(DateOnly fecha, IEnumerable<string> legajos)
    {
        Fecha = fecha;
        _estados = legajos.ToDictionary(l => l, _ => true); // CA 3.1: presente por defecto
    }

    public void MarcarAusente(string legajo) => Cambiar(legajo, false);
    public void MarcarPresente(string legajo) => Cambiar(legajo, true);

    private void Cambiar(string legajo, bool presente)
    {
        if (!_estados.ContainsKey(legajo))
            throw new KeyNotFoundException($"El legajo {legajo} no está en la lista.");
        _estados[legajo] = presente;
    }
}

public class ServicioAsistencia
{
    private readonly Dictionary<DateOnly, IReadOnlyDictionary<string, bool>> _guardadas = new();

    public bool ExisteRegistro(DateOnly fecha) => _guardadas.ContainsKey(fecha);

    public ListaAsistencia IniciarToma(DateOnly fecha, Nomina nomina)
    {
        ArgumentNullException.ThrowIfNull(nomina);

        if (ExisteRegistro(fecha))
            throw new AsistenciaYaRegistradaException(fecha); // CA 3.4

        var activos = nomina.Activos.Select(e => e.Legajo).ToList();
        if (activos.Count == 0)
            throw new InvalidOperationException("No hay estudiantes activos en la comisión.");

        return new ListaAsistencia(fecha, activos);
    }

    public void Guardar(ListaAsistencia lista)
    {
        ArgumentNullException.ThrowIfNull(lista);

        if (ExisteRegistro(lista.Fecha))
            throw new AsistenciaYaRegistradaException(lista.Fecha);

        _guardadas[lista.Fecha] = new Dictionary<string, bool>(lista.Estados); // copia
    }

    public IReadOnlyDictionary<string, bool>? Obtener(DateOnly fecha) =>
        _guardadas.TryGetValue(fecha, out var estados) ? estados : null;
}