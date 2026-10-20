namespace App_v3.Models;

public class ReporteAsistenciaItem
{
    public int EstudianteId { get; set; }
    public int ClaseId { get; set; }
    public string NombreEstudiante { get; set; } = string.Empty;
    public string Dni { get; set; } = string.Empty;
    public string MateriaClase { get; set; } = string.Empty;
    public string Comision { get; set; } = string.Empty;
    public DateTime FechaClase { get; set; }
    public bool Presente { get; set; }
    public bool Justificada { get; set; }

    // Propiedades visuales calculadas para los colores y textos
    public string EstadoTexto => Presente ? "Presente" : (Justificada ? "Justificado" : "Ausente");
    public string ColorEstado => Presente ? "#2E7D32" : (Justificada ? "#F57C00" : "#D32F2F"); // Verde, Naranja, Rojo
}