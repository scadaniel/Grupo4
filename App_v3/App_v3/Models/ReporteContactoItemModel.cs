namespace App_v3.Models
{
    public class ReporteContactoItemModel
    {
        public string NombreEstudiante { get; set; } = string.Empty;
        public string Legajo { get; set; } = string.Empty;
        public string ComisionNombre { get; set; } = string.Empty; // <-- Agregá esta línea
        public DateTime FechaContacto { get; set; }
        public string MedioContacto { get; set; } = string.Empty;
        public string Motivo { get; set; } = string.Empty;
        public string ReporteSituacion { get; set; } = string.Empty;
        public int AsistenciasAlMomento { get; set; }
        public int TpsNoEntregadosAlMomento { get; set; }
        public string? Mensaje { get; set; }

        // Propiedades formateadas para la vista
        public string FechaTexto => FechaContacto.ToString("dd/MM/yyyy HH:mm");
        public string DetalleAcademico => $"Inasistencias: {AsistenciasAlMomento} | TPs Pendientes: {TpsNoEntregadosAlMomento}";
    }
}