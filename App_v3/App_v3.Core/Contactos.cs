using App_v3.Models;

namespace App_v3.Core;

// Historias H10 y H11: seguimiento de los contactos con estudiantes en riesgo.
// Este archivo es dominio puro: no conoce SQLite ni las pantallas.
// Para guardar usa IContactoStore, que la app implementa con la base de datos.

public enum MedioContacto
{
    Llamada,
    Email
}

// Lo que el responsable completa en el formulario. La fecha, el resultado y la nota
// pueden venir vacíos y se controlan recién al registrar.
public record NuevoContacto(
    int EstudianteId,
    DateTime? Fecha,
    MedioContacto Medio,
    string? Resultado,
    string? Nota);

// Un contacto ya guardado en el historial del estudiante.
public record Contacto(
    int EstudianteId,
    DateTime Fecha,
    MedioContacto Medio,
    string Resultado,
    string Nota);

public class ContactoInvalidoException(IReadOnlyList<string> camposFaltantes)
    : ArgumentException("Faltan datos obligatorios: " + string.Join(", ", camposFaltantes))
{
    public IReadOnlyList<string> CamposFaltantes { get; } = camposFaltantes;
}

public interface IContactoStore
{
    Task GuardarAsync(Contacto contacto);
    Task<IReadOnlyList<Contacto>> ListarAsync();
}

public class SeguimientoContactos
{
    // Un contacto de hace 7 días o menos cuenta como "de esta semana".
    private const int DiasParaSerReciente = 7;

    private readonly IContactoStore _store;
    private readonly Func<DateTime> _ahora;

    // El reloj se puede reemplazar para que las pruebas no dependan del día en que se corren.
    public SeguimientoContactos(IContactoStore store, Func<DateTime>? ahora = null)
    {
        _store = store;
        _ahora = ahora ?? (() => DateTime.Now);
    }

    // Criterios 10.1 y 10.2: guarda el contacto, o avisa qué campos faltan y no guarda nada.
    public async Task RegistrarAsync(NuevoContacto nuevo)
    {
        ArgumentNullException.ThrowIfNull(nuevo);

        var faltantes = CamposFaltantes(nuevo);
        if (faltantes.Count > 0)
            throw new ContactoInvalidoException(faltantes);

        await _store.GuardarAsync(new Contacto(
            nuevo.EstudianteId,
            nuevo.Fecha!.Value,
            nuevo.Medio,
            nuevo.Resultado!.Trim(),
            nuevo.Nota!.Trim()));
    }

    // Criterio 11.1: fecha del último contacto, o null si nunca se contactó al estudiante.
    public async Task<DateTime?> UltimoContactoAsync(int estudianteId)
    {
        var contactos = await _store.ListarAsync();
        return contactos
            .Where(c => c.EstudianteId == estudianteId)
            .Select(c => (DateTime?)c.Fecha)
            .Max();
    }

    // Criterio 11.1 para todo el panel de una sola vez: la fecha del último contacto
    // de cada estudiante. Los que nunca fueron contactados no aparecen en el resultado.
    public async Task<Dictionary<int, DateTime>> UltimosContactosAsync()
    {
        var contactos = await _store.ListarAsync();
        return contactos
            .GroupBy(c => c.EstudianteId)
            .ToDictionary(g => g.Key, g => g.Max(c => c.Fecha));
    }

    // Criterio 11.2: de los estudiantes del panel, devuelve los que tienen riesgo Medio o Alto
    // y no tuvieron ningún contacto en los últimos 7 días.
    public async Task<List<int>> NoContactadosEstaSemanaAsync(
        IEnumerable<(int EstudianteId, NivelRiesgo Nivel)> estudiantes)
    {
        var desde = _ahora().Date.AddDays(-DiasParaSerReciente);
        var contactos = await _store.ListarAsync();

        var contactadosRecientemente = contactos
            .Where(c => c.Fecha.Date >= desde)
            .Select(c => c.EstudianteId)
            .ToHashSet();

        return estudiantes
            .Where(e => e.Nivel != NivelRiesgo.Bajo && !contactadosRecientemente.Contains(e.EstudianteId))
            .Select(e => e.EstudianteId)
            .ToList();
    }

    private static List<string> CamposFaltantes(NuevoContacto contacto)
    {
        var faltantes = new List<string>();

        if (contacto.Fecha is null) faltantes.Add("Fecha");
        if (string.IsNullOrWhiteSpace(contacto.Resultado)) faltantes.Add("Resultado");
        if (string.IsNullOrWhiteSpace(contacto.Nota)) faltantes.Add("Nota");

        return faltantes;
    }
}
