using App_v3.Models;
using App_v3.Services;

namespace App_v3.ViewModels
{
    public class EstudianteItemViewModel : BindableObject
    {
        public Estudiante Estudiante { get; set; }

        // Propiedades directas expuestas para evitar errores en enlaces y ordenamientos
        public string Legajo => Estudiante?.Legajo ?? string.Empty;
        public string NombreCompleto => Estudiante?.NombreCompleto ?? string.Empty;
        public string Email => Estudiante?.Email ?? string.Empty;
        public string Dni => Estudiante?.Dni ?? string.Empty;
        public string Telefono => Estudiante?.Telefono ?? string.Empty;

        // Propiedades anteriores conservadas por compatibilidad
        public double Promedio { get; set; }
        public int PorcentajeAsistencia { get; set; }
        public bool EnRiesgo => Promedio < 6.0 || PorcentajeAsistencia < 75;

        // --- Nuevas propiedades para el Motor de Riesgo (H6, H8) ---
        private NivelRiesgo _nivelRiesgoEnum = NivelRiesgo.Bajo;
        public NivelRiesgo NivelRiesgoEnum
        {
            get => _nivelRiesgoEnum;
            set { _nivelRiesgoEnum = value; OnPropertyChanged(); }
        }

        private string _colorRiesgoHex = "#388E3C";
        public string ColorRiesgoHex
        {
            get => _colorRiesgoHex;
            set { _colorRiesgoHex = value; OnPropertyChanged(); }
        }

        private string _diagnosticoRiesgo = string.Empty;
        public string DiagnosticoRiesgo
        {
            get => _diagnosticoRiesgo;
            set { _diagnosticoRiesgo = value; OnPropertyChanged(); }
        }

        // --- CONSTRUCTORES ---

        // Constructor original (para no romper otras pantallas que lo usen con 3 argumentos)
        public EstudianteItemViewModel(Estudiante estudiante, double promedio, int porcentajeAsistencia)
        {
            Estudiante = estudiante;
            Promedio = promedio;
            PorcentajeAsistencia = porcentajeAsistencia;
        }

        // Nuevo constructor requerido por el EstudianteListViewModel actualizado (H8)
        public EstudianteItemViewModel(Estudiante estudiante, NivelRiesgo nivelRiesgo, string colorHex, string diagnostico)
        {
            Estudiante = estudiante;
            _nivelRiesgoEnum = nivelRiesgo;
            _colorRiesgoHex = colorHex;
            _diagnosticoRiesgo = diagnostico;

            // Valores por defecto seguros para las propiedades viejas si se inicializa desde aquí
            Promedio = 7.0;
            PorcentajeAsistencia = 85;
        }
    }
}