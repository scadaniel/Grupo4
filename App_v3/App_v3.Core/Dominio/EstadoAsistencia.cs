namespace App_v3.Dominio;

/// <summary>
/// H3 y H4: cómo figura un estudiante en una clase.
/// La base guarda dos bool (Presente y Justificada); este enum es solo la forma de nombrarlos.
/// </summary>
public enum EstadoAsistencia
{
    Presente,
    Ausente,
    AusenteJustificado
}
