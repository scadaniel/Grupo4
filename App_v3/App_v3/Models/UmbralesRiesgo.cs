namespace App_v3.Models;

public class UmbralesRiesgo
{
    // --- PROPIEDADES ANTERIORES / ESTÁNDAR ---
    // (Se mantienen para asegurar que cualquier otra pantalla o servicio existente siga compilando sin problemas)
    public int InasistenciasBajoLimite { get; set; } = 2;
    public int InasistenciasMedioLimite { get; set; } = 4;

    public int TpsBajoLimite { get; set; } = 1;
    public int TpsMedioLimite { get; set; } = 2;

    // --- VALIDACIÓN DE UMBRALES (CA 7.3) ---
    /// <summary>
    /// Valida que los valores de los umbrales no sean contradictorios ni negativos.
    /// </summary>
    public (bool EsValido, string MensajeError) Validar()
    {
        if (InasistenciasBajoLimite < 0 || InasistenciasMedioLimite < 0 || TpsBajoLimite < 0 || TpsMedioLimite < 0)
        {
            return (false, "Los umbrales no pueden contener valores negativos.");
        }

        if (InasistenciasMedioLimite <= InasistenciasBajoLimite)
        {
            return (false, "El límite medio de inasistencias debe ser mayor estricto al límite bajo.");
        }

        if (TpsMedioLimite <= TpsBajoLimite)
        {
            return (false, "El límite medio de trabajos prácticos debe ser mayor estricto al límite bajo.");
        }

        return (true, string.Empty);
    }
}
