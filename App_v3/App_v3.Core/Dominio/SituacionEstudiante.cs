namespace App_v3.Dominio;

/// <summary>
/// H6 y H8: lo que el panel de seguimiento sabe de un estudiante activo.
/// El nivel se calcula en el momento con UmbralesRiesgo.CalcularNivel; nunca se guarda.
/// </summary>
public record SituacionEstudiante(
    Estudiante Estudiante,
    NivelRiesgo Nivel,
    int Inasistencias,
    int TpsVencidosNoEntregados,
    DateTime? UltimoContacto)
{
    /// <summary>CA 8.2 y CA 9.2: los datos que justifican el nivel, escritos en una frase.</summary>
    public string Motivo => ArmarMotivo(Inasistencias, TpsVencidosNoEntregados);

    private static string ArmarMotivo(int inasistencias, int tps)
    {
        var partes = new List<string>();
        if (inasistencias > 0) partes.Add($"{inasistencias} inasistencia{Plural(inasistencias)}");
        if (tps > 0) partes.Add($"{tps} TP{Plural(tps)} vencido{Plural(tps)} sin entregar");

        return partes.Count == 0
            ? "Sin inasistencias ni TPs vencidos sin entregar"
            : string.Join(" y ", partes);
    }

    private static string Plural(int cantidad) => cantidad == 1 ? string.Empty : "s";
}
