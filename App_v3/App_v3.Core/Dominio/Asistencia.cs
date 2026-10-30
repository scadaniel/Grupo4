namespace App_v3.Dominio;

/// <summary>
/// H3 y H4: asistencia de un estudiante a una clase dictada.
/// Presente = false es Ausente. Una ausencia justificada no suma al riesgo (CA 3.5).
/// </summary>
public class Asistencia : IEntidad
{
    public int Id { get; set; }
    public int ClaseDictadaId { get; set; }
    public int EstudianteId { get; set; }
    public bool Presente { get; set; } = true;
    public bool Justificada { get; set; }

    /// <summary>Única forma de convertir los dos bool en un estado.</summary>
    public EstadoAsistencia Estado => Presente
        ? EstadoAsistencia.Presente
        : Justificada ? EstadoAsistencia.AusenteJustificado : EstadoAsistencia.Ausente;

    /// <summary>Una ausencia sin justificar es la que suma al indicador de riesgo.</summary>
    public bool CuentaComoInasistencia => Estado == EstadoAsistencia.Ausente;
}
