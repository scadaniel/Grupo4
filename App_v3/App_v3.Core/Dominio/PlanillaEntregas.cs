namespace App_v3.Dominio;

/// <summary>Un estudiante en la planilla de entregas de un trabajo práctico (H5).</summary>
public record LineaEntrega(Estudiante Estudiante, bool Entregado);

/// <summary>
/// La planilla de un trabajo: todos los estudiantes activos de la comisión.
/// Si ya hay información guardada, YaRegistrada es true y las líneas traen el estado guardado (CA 5.4).
/// </summary>
public record PlanillaEntregas(TrabajoPractico Trabajo, bool YaRegistrada, IReadOnlyList<LineaEntrega> Lineas);
