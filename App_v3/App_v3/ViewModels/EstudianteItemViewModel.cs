using App_v3.Models;

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

        // --- Propiedades para el Motor de Riesgo ---
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

        // --- ALIAS DE COMPATIBILIDAD PARA EL XAML ---
        public string ColorHex => ColorRiesgoHex;
        public string Diagnostico => DiagnosticoRiesgo;
        public string NivelRiesgoText => NivelRiesgoEnum.ToString().ToUpper();

        // --- CONSTRUCTORES ---

        // Constructor 1: Para otras pantallas que pasen promedio y porcentaje
        public EstudianteItemViewModel(Estudiante estudiante, double promedio, int porcentajeAsistencia)
        {
            Estudiante = estudiante;
            Promedio = promedio;
            PorcentajeAsistencia = porcentajeAsistencia;
        }

        // Constructor 2: Requerido por el EstudianteListViewModel (Riesgo personalizado)
        public EstudianteItemViewModel(Estudiante estudiante, NivelRiesgo nivelRiesgo, string colorHex, string diagnostico)
        {
            Estudiante = estudiante;
            _nivelRiesgoEnum = nivelRiesgo;
            _colorRiesgoHex = colorHex;
            _diagnosticoRiesgo = diagnostico;

            Promedio = 7.0;
            PorcentajeAsistencia = 85;
        }
    }

    // Definición del Enum NivelRiesgo para evitar errores de tipo no encontrado
    public enum NivelRiesgo
    {
        Bajo,
        Medio,
        Alto
    }
}