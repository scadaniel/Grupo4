namespace App_v3.Core;

public record FichaEstudiante(
    string Legajo, string Dni, string Nombre, string Apellido, string Email, string? Telefono = null)
{
    public bool Activo { get; init; } = true;
}

public class EstudianteDuplicadoException(string mensaje) : InvalidOperationException(mensaje);

public class Nomina
{
    private readonly List<FichaEstudiante> _estudiantes = new();

    public IReadOnlyList<FichaEstudiante> Todos => _estudiantes;
    public IReadOnlyList<FichaEstudiante> Activos => _estudiantes.Where(e => e.Activo).ToList();

    public void Alta(FichaEstudiante estudiante)
    {
        ArgumentNullException.ThrowIfNull(estudiante);

        foreach (var campo in new[] { estudiante.Legajo, estudiante.Dni, estudiante.Nombre, estudiante.Apellido, estudiante.Email })
            if (string.IsNullOrWhiteSpace(campo))
                throw new ArgumentException("Faltan campos obligatorios.");

        if (_estudiantes.Any(e => e.Legajo == estudiante.Legajo || e.Dni == estudiante.Dni))
            throw new EstudianteDuplicadoException("Ya existe un estudiante con ese legajo o DNI.");

        _estudiantes.Add(estudiante);
    }

    public void Inactivar(string legajo)
    {
        var i = _estudiantes.FindIndex(e => e.Legajo == legajo);
        if (i < 0) throw new KeyNotFoundException($"No existe el legajo {legajo}.");
        _estudiantes[i] = _estudiantes[i] with { Activo = false };
    }
}