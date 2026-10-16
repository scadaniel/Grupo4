using App_v3.Models;

namespace App_v3.Services;

public class ConfiguracionService
{
    private const string KeyInasistenciasBajo = "Umbral_Inasistencias_Bajo";
    private const string KeyInasistenciasMedio = "Umbral_Inasistencias_Medio";
    private const string KeyTpsBajo = "Umbral_TPs_Bajo";
    private const string KeyTpsMedio = "Umbral_TPs_Medio";

    public UmbralesRiesgo ObtenerUmbrales()
    {
        return new UmbralesRiesgo
        {
            InasistenciasBajoLimite = Preferences.Get(KeyInasistenciasBajo, 2),
            InasistenciasMedioLimite = Preferences.Get(KeyInasistenciasMedio, 4),
            TpsBajoLimite = Preferences.Get(KeyTpsBajo, 1),
            TpsMedioLimite = Preferences.Get(KeyTpsMedio, 2)
        };
    }

    public (bool Exito, string Mensaje) GuardarUmbrales(UmbralesRiesgo umbrales)
    {
        // Validación obligatoria según CA 7.3
        var validacion = umbrales.Validar();
        if (!validacion.EsValido)
        {
            return (false, validacion.MensajeError);
        }

        Preferences.Set(KeyInasistenciasBajo, umbrales.InasistenciasBajoLimite);
        Preferences.Set(KeyInasistenciasMedio, umbrales.InasistenciasMedioLimite);
        Preferences.Set(KeyTpsBajo, umbrales.TpsBajoLimite);
        Preferences.Set(KeyTpsMedio, umbrales.TpsMedioLimite);

        return (true, "Umbrales actualizados correctamente.");
    }
}