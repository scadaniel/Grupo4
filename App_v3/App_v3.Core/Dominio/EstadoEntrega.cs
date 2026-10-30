namespace App_v3.Dominio;

/// <summary>
/// Cómo figura un trabajo práctico para un estudiante.
/// Pendiente es "todavía no entregó, pero la fecha límite no pasó": no cuenta para el riesgo.
/// </summary>
public enum EstadoEntrega
{
    Entregado,
    NoEntregado,
    Pendiente
}

/// <summary>Regla única para decidir el estado de una entrega y si cuenta para el riesgo (H6).</summary>
internal static class ReglasDeEntrega
{
    public static bool EstaVencido(TrabajoPractico trabajo, DateTime hoy) => trabajo.FechaLimite.Date < hoy.Date;

    public static EstadoEntrega EstadoDe(TrabajoPractico trabajo, Entrega? entrega, DateTime hoy)
    {
        if (entrega is { Entregado: true }) return EstadoEntrega.Entregado;
        return EstaVencido(trabajo, hoy) ? EstadoEntrega.NoEntregado : EstadoEntrega.Pendiente;
    }
}
