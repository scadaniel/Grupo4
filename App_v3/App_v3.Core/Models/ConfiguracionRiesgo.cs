using SQLite;

namespace App_v3.Models;

[Table("ConfiguracionRiesgo")]
public class ConfiguracionRiesgo : IEntidad
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // Inasistencias a partir de las cuales el riesgo es Medio / Alto
    public int InasistenciasMedio { get; set; } = 3;
    public int InasistenciasAlto { get; set; } = 5;

    // TPs vencidos no entregados a partir de los cuales el riesgo es Medio / Alto
    public int TpsNoEntregadosMedio { get; set; } = 1;
    public int TpsNoEntregadosAlto { get; set; } = 3;

    // CA 7.3: valida que los umbrales no sean contradictorios
    public bool EsValida(out string mensaje)
    {
        if (InasistenciasMedio < 1 || InasistenciasAlto < 1 ||
            TpsNoEntregadosMedio < 1 || TpsNoEntregadosAlto < 1)
        {
            mensaje = "Los umbrales deben ser mayores o iguales a 1.";
            return false;
        }

        if (InasistenciasMedio >= InasistenciasAlto)
        {
            mensaje = "El umbral Medio de inasistencias debe ser menor que el Alto.";
            return false;
        }

        if (TpsNoEntregadosMedio >= TpsNoEntregadosAlto)
        {
            mensaje = "El umbral Medio de TPs no entregados debe ser menor que el Alto.";
            return false;
        }

        mensaje = string.Empty;
        return true;
    }

    // CA 7.2: aplica los umbrales a cantidades ya contadas.
    // El nivel general es el más alto de las dos señales (como pide H6).
    public NivelRiesgo CalcularNivel(int inasistencias, int tpsNoEntregados)
    {
        var porInasistencias = NivelPorSenal(inasistencias, InasistenciasMedio, InasistenciasAlto);
        var porTps = NivelPorSenal(tpsNoEntregados, TpsNoEntregadosMedio, TpsNoEntregadosAlto);

        return (NivelRiesgo)Math.Max((int)porInasistencias, (int)porTps);
    }

    private static NivelRiesgo NivelPorSenal(int cantidad, int umbralMedio, int umbralAlto)
    {
        if (cantidad >= umbralAlto) return NivelRiesgo.Alto;
        if (cantidad >= umbralMedio) return NivelRiesgo.Medio;
        return NivelRiesgo.Bajo;
    }
}
