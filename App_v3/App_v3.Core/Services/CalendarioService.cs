using App_v3.Models;

namespace App_v3.Services;

/// <summary>Error de validación con un mensaje listo para mostrar al usuario.</summary>
public class DatosInvalidosException : Exception
{
    public DatosInvalidosException(string mensaje) : base(mensaje) { }
}

/// <summary>Un elemento del calendario: una clase o un trabajo práctico.</summary>
public class EventoCalendario
{
    public int Id { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
}

/// <summary>H2: reglas del calendario de clases y de trabajos prácticos.</summary>
public class CalendarioService
{
    public const string TipoClase = "Clase";
    public const string TipoTrabajo = "Trabajo Práctico";

    private readonly ICalendarioStore _store;

    public CalendarioService(ICalendarioStore store) => _store = store;

    /// <summary>Arma el servicio sobre la base de datos real de la app.</summary>
    public static CalendarioService Para(IDatabaseService db) =>
        new(new CalendarioStoreSobreBaseDeDatos(db));

    // CA 2.3: clases y trabajos juntos, en orden cronológico
    public async Task<List<EventoCalendario>> ListarEventosAsync()
    {
        var eventos = new List<EventoCalendario>();

        foreach (var c in await _store.GetClasesAsync())
        {
            eventos.Add(new EventoCalendario
            {
                Id = c.Id,
                Tipo = TipoClase,
                Descripcion = string.IsNullOrWhiteSpace(c.Tema)
                    ? $"{c.NombreMateria} (Comisión {c.Comision})"
                    : $"Clase {c.NumeroClase}: {c.Tema}",
                Fecha = c.Fecha
            });
        }

        foreach (var tp in await _store.GetTrabajosPracticosAsync())
        {
            eventos.Add(new EventoCalendario
            {
                Id = tp.Id,
                Tipo = TipoTrabajo,
                Descripcion = string.IsNullOrWhiteSpace(tp.Descripcion)
                    ? tp.Nombre
                    : $"{tp.Nombre} - {tp.Descripcion}",
                Fecha = tp.FechaLimite
            });
        }

        return eventos.OrderBy(e => e.Fecha).ToList();
    }

    // CA 2.1, CA 2.2 y CA 2.4
    public async Task AgregarEventoAsync(string? tipo, string? titulo, DateTime fecha)
    {
        if (string.IsNullOrWhiteSpace(tipo) || string.IsNullOrWhiteSpace(titulo))
            throw new DatosInvalidosException(
                "Por favor, complete todos los campos obligatorios (Tipo y Título/Tema) antes de guardar.");

        if (tipo == TipoClase)
        {
            int proximoNumero = (await _store.GetClasesAsync()).Count + 1;
            await _store.SaveClaseAsync(new Clase
            {
                NombreMateria = "Materia General",
                Comision = "Única",
                NumeroClase = proximoNumero,
                Tema = titulo.Trim(),
                Fecha = fecha
            });
        }
        else if (tipo == TipoTrabajo)
        {
            await _store.SaveTrabajoPracticoAsync(new TrabajoPractico
            {
                Nombre = titulo.Trim(),
                FechaLimite = fecha
            });
        }
        else
        {
            throw new DatosInvalidosException($"El tipo de evento \"{tipo}\" no es válido.");
        }
    }
}
