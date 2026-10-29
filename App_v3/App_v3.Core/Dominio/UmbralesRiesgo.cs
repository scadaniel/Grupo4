namespace App_v3.Dominio;

/// <summary>
/// H7: valores que deciden cuándo el riesgo es Medio o Alto.
/// Es el único lugar donde se calcula el nivel (H6): cada señal da un nivel
/// y el general es el más alto de las dos.
/// </summary>
public class UmbralesRiesgo : IEntidad
{
    public int Id { get; set; }

    public int InasistenciasMedio { get; set; } = 3;
    public int InasistenciasAlto { get; set; } = 5;
    public int TpsNoEntregadosMedio { get; set; } = 1;
    public int TpsNoEntregadosAlto { get; set; } = 3;

    // CA 7.3: los valores contradictorios no se pueden guardar.
    public bool EsValida(out string mensaje)
    {
        mensaje = Validar();
        return mensaje.Length == 0;
    }

    // CA 6.1 y 6.2
    public NivelRiesgo CalcularNivel(int inasistencias, int tpsNoEntregados)
    {
        var porInasistencias = NivelPorSenal(inasistencias, InasistenciasMedio, InasistenciasAlto);
        var porTps = NivelPorSenal(tpsNoEntregados, TpsNoEntregadosMedio, TpsNoEntregadosAlto);
        return (NivelRiesgo)Math.Max((int)porInasistencias, (int)porTps);
    }

    private string Validar()
    {
        int[] todos = [InasistenciasMedio, InasistenciasAlto, TpsNoEntregadosMedio, TpsNoEntregadosAlto];
        if (todos.Any(umbral => umbral < 1))
            return "Los umbrales deben ser mayores o iguales a 1.";
        if (InasistenciasMedio >= InasistenciasAlto)
            return "El umbral Medio de inasistencias debe ser menor que el Alto.";
        if (TpsNoEntregadosMedio >= TpsNoEntregadosAlto)
            return "El umbral Medio de TPs no entregados debe ser menor que el Alto.";
        return string.Empty;
    }

    private static NivelRiesgo NivelPorSenal(int cantidad, int umbralMedio, int umbralAlto)
    {
        if (cantidad >= umbralAlto) return NivelRiesgo.Alto;
        if (cantidad >= umbralMedio) return NivelRiesgo.Medio;
        return NivelRiesgo.Bajo;
    }
}
