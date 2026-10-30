namespace App_v3.Dominio;

/// <summary>H12 CA 12.1: una clase y cómo figura el estudiante en ella.</summary>
public record RegistroDeAsistencia(ClaseDictada Clase, EstadoAsistencia Estado);

/// <summary>H12 CA 12.2: un trabajo práctico y cómo figura el estudiante en él.</summary>
public record RegistroDeEntrega(TrabajoPractico Trabajo, EstadoEntrega Estado);

/// <summary>
/// H9, H12 y H13: ficha y trayectoria de un estudiante en una comisión.
/// Para un estudiante inactivo conserva todo su historial; el nivel de riesgo es null porque ya no está en el panel.
/// </summary>
public record FichaEstudiante(
    Estudiante Estudiante,
    bool Activo,
    DateTime? FechaBaja,
    NivelRiesgo? Nivel,
    string Motivo,
    IReadOnlyList<RegistroDeAsistencia> Asistencias,
    IReadOnlyList<RegistroDeEntrega> Entregas,
    IReadOnlyList<Contacto> Contactos)
{
    /// <summary>CA 12.3</summary>
    public int TotalInasistencias => Asistencias.Count(a => a.Estado == EstadoAsistencia.Ausente);

    /// <summary>CA 12.3</summary>
    public int TotalTpsNoEntregados => Entregas.Count(e => e.Estado == EstadoEntrega.NoEntregado);
}
