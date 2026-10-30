namespace App_v3.Dominio;

/// <summary>Un estudiante en la grilla de toma de asistencia (H3) o de corrección (H4).</summary>
public record LineaAsistencia(Estudiante Estudiante, bool Presente = true, bool Justificada = false);

/// <summary>
/// La grilla de una clase: todos los estudiantes activos de la comisión.
/// Si ya hay asistencia guardada, YaRegistrada es true y las líneas traen los estados guardados.
/// </summary>
public record PlanillaAsistencia(ClaseDictada Clase, bool YaRegistrada, IReadOnlyList<LineaAsistencia> Lineas);
