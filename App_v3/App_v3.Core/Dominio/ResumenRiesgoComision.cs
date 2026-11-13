namespace App_v3.Dominio;

/// <summary>H12: qué se pudo calcular para una comisión. "SinDatos" NO es lo mismo que "0 en riesgo".</summary>
public enum EstadoResumenRiesgo
{
    /// <summary>La comisión no tiene estudiantes activos.</summary>
    SinEstudiantes,

    /// <summary>Hay estudiantes pero no hay ninguna asistencia ni entrega registrada (CA3).</summary>
    SinDatos,

    /// <summary>Hay datos y el riesgo se calculó (CA1).</summary>
    Calculado
}

/// <summary>
/// H12: cuántos estudiantes activos de la comisión están en riesgo de abandonar.
/// "En riesgo" es el nivel Medio o Alto del panel de seguimiento (el mismo cálculo, CA2).
/// </summary>
public record ResumenRiesgoComision(EstadoResumenRiesgo Estado, int EnRiesgo, int TotalActivos, int Alto, int Medio)
{
    public static ResumenRiesgoComision SinEstudiantes() => new(EstadoResumenRiesgo.SinEstudiantes, 0, 0, 0, 0);

    public static ResumenRiesgoComision SinDatos(int totalActivos) => new(EstadoResumenRiesgo.SinDatos, 0, totalActivos, 0, 0);
}
