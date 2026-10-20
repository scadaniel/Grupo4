namespace App_v3.Models;

public static class RiesgoAcademico
{
    public const double PromedioMinimo = 6.0;
    public const int AsistenciaMinima = 75;

    public static bool EnRiesgo(double promedio, int porcentajeAsistencia)
        => promedio < PromedioMinimo || porcentajeAsistencia < AsistenciaMinima;
}