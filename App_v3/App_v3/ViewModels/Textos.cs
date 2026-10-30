using App_v3.Dominio;

namespace App_v3.ViewModels;

/// <summary>Cómo se escribe cada estado en pantalla. Un solo lugar para todos los textos.</summary>
public static class Textos
{
    public static string De(NivelRiesgo nivel) => nivel switch
    {
        NivelRiesgo.Alto => "ALTO",
        NivelRiesgo.Medio => "MEDIO",
        _ => "BAJO"
    };

    public static string De(EstadoAsistencia estado) => estado switch
    {
        EstadoAsistencia.Presente => "Presente",
        EstadoAsistencia.AusenteJustificado => "Ausente justificado",
        _ => "Ausente"
    };

    public static string De(EstadoEntrega estado) => estado switch
    {
        EstadoEntrega.Entregado => "Entregado",
        EstadoEntrega.NoEntregado => "No entregado",
        _ => "Pendiente"
    };

    public static string De(MedioContacto medio) => medio == MedioContacto.Email ? "Email" : "Llamada";

    public static string Fecha(DateTime fecha) => fecha.ToString("dd/MM/yyyy");

    public static string De(ClaseDictada clase) => $"Clase {clase.Numero} · {Fecha(clase.Fecha)} · {clase.Tema}";

    public static string De(TrabajoPractico trabajo) => $"{trabajo.Nombre} · vence {Fecha(trabajo.FechaLimite)}";
}

/// <summary>
/// Qué color semántico corresponde a cada estado. Devuelve el nombre del color en Colors.xaml:
/// los códigos hexadecimales existen únicamente ahí.
/// </summary>
public static class PaletaSemantica
{
    public const string Exito = "ColorExito";
    public const string Peligro = "ColorPeligro";
    public const string Advertencia = "ColorAdvertencia";
    public const string Neutro = "ColorNeutro";

    public static string ClaveDe(object? estado) => estado switch
    {
        NivelRiesgo.Alto => Peligro,
        NivelRiesgo.Medio => Advertencia,
        NivelRiesgo.Bajo => Exito,
        EstadoAsistencia.Presente => Exito,
        EstadoAsistencia.AusenteJustificado => Advertencia,
        EstadoAsistencia.Ausente => Peligro,
        EstadoEntrega.Entregado => Exito,
        EstadoEntrega.NoEntregado => Peligro,
        EstadoEntrega.Pendiente => Neutro,
        bool activo => activo ? Exito : Peligro,
        _ => Neutro
    };
}
