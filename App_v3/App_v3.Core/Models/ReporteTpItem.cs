namespace App_v3.Models;

public class ReporteTpItem
{
    public int EstudianteId { get; set; }
    public int TrabajoPracticoId { get; set; }
    public string NombreEstudiante { get; set; } = string.Empty;
    public string Dni { get; set; } = string.Empty;
    public string NombreTp { get; set; } = string.Empty;
    public string DescripcionTp { get; set; } = string.Empty;
    public bool Entregado { get; set; }
    public DateTime? FechaEntrega { get; set; }

    // Propiedades visuales calculadas para colores y textos
    public string EstadoTexto => Entregado ? "Entregado" : "Pendiente";
    public string ColorEstado => Entregado ? "#2E7D32" : "#D32F2F"; // Verde o Rojo
    public string TextoFechaEntrega => FechaEntrega.HasValue
        ? $"Entregado el: {FechaEntrega.Value:dd/MM/yyyy}"
        : "Sin entrega registrada";
}