namespace App_v3.Dominio;

/// <summary>
/// Un estudiante cursando una comisión. La baja es lógica (H1 CA 1.4, H13):
/// se marca Activa = false y se conserva todo el historial.
/// </summary>
public class Inscripcion : IEntidad
{
    public int Id { get; set; }
    public int EstudianteId { get; set; }
    public int ComisionId { get; set; }
    public bool Activa { get; set; } = true;
    public DateTime? FechaBaja { get; set; }
}
