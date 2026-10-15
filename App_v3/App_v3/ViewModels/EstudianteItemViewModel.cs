using App_v3.Models;

namespace App_v3.ViewModels
{
    public class EstudianteItemViewModel : BindableObject
    {
        public Estudiante Estudiante { get; set; }
        public double Promedio { get; set; }
        public int PorcentajeAsistencia { get; set; }

        // Regla de negocio para determinar si un alumno está en riesgo académico
        // (Ejemplo: Promedio menor a 6.0 O Asistencia menor al 75%)
        public bool EnRiesgo => Promedio < 6.0 || PorcentajeAsistencia < 75;

        public EstudianteItemViewModel(Estudiante estudiante, double promedio, int porcentajeAsistencia)
        {
            Estudiante = estudiante;
            Promedio = promedio;
            PorcentajeAsistencia = porcentajeAsistencia;
        }
    }
}