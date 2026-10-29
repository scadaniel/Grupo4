using App_v3.Models;

namespace App_v3.Services;

public class RiesgoService
{
    /// <summary>
    /// Sobrecarga práctica: Calcula el riesgo leyendo automáticamente los umbrales de las preferencias o valores por defecto.
    /// </summary>
    public static (NivelRiesgo Nivel, string ColorHex, string Diagnostico) CalcularRiesgo(
        int inasistencias,
        int tpsNoEntregados)
    {
        // Si no se pasan umbrales, los lee directo de Preferences (H7) con valores seguros por defecto
        var umbrales = new UmbralesRiesgo
        {
            InasistenciasBajoLimite = Preferences.Get("Umbral_Inasistencias_Bajo", 2),
            InasistenciasMedioLimite = Preferences.Get("Umbral_Inasistencias_Medio", 4),
            TpsBajoLimite = Preferences.Get("Umbral_TPs_Bajo", 1),
            TpsMedioLimite = Preferences.Get("Umbral_TPs_Medio", 2)
        };

        return CalcularRiesgo(inasistencias, tpsNoEntregados, umbrales);
    }

    /// <summary>
    /// CA 6.1 y CA 6.2: Evalúa inasistencias y TPs vencidos asignando el nivel general más alto.
    /// (Tu método original intacto)
    /// </summary>
    public static (NivelRiesgo Nivel, string ColorHex, string Diagnostico) CalcularRiesgo(
        int inasistencias,
        int tpsNoEntregados,
        UmbralesRiesgo umbrales)
    {
        // 1. Evaluar Riesgo por Inasistencias
        NivelRiesgo riesgoAsistencia;
        if (inasistencias <= umbrales.InasistenciasBajoLimite)
            riesgoAsistencia = NivelRiesgo.Bajo;
        else if (inasistencias <= umbrales.InasistenciasMedioLimite)
            riesgoAsistencia = NivelRiesgo.Medio;
        else
            riesgoAsistencia = NivelRiesgo.Alto;

        // 2. Evaluar Riesgo por TPs No Entregados
        NivelRiesgo riesgoTPs;
        if (tpsNoEntregados <= umbrales.TpsBajoLimite)
            riesgoTPs = NivelRiesgo.Bajo;
        else if (tpsNoEntregados <= umbrales.TpsMedioLimite)
            riesgoTPs = NivelRiesgo.Medio;
        else
            riesgoTPs = NivelRiesgo.Alto;

        // 3. Tomar el nivel más alto entre las dos señales (CA 6.2)
        NivelRiesgo riesgoGeneral = (NivelRiesgo)Math.Max((int)riesgoAsistencia, (int)riesgoTPs);

        // Mapas de colores e información para la UI
        return riesgoGeneral switch
        {
            NivelRiesgo.Alto => (NivelRiesgo.Alto, "#D32F2F", "Riesgo Alto - Requiere Intervención"),
            NivelRiesgo.Medio => (NivelRiesgo.Medio, "#F57C00", "Riesgo Medio - En Advertencia"),
            _ => (NivelRiesgo.Bajo, "#388E3C", "Riesgo Bajo - Situación Normal")
        };
    }
}