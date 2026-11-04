using App_v3.Models;

namespace App_v3.ViewModels
{
    public class EstudianteItemViewModel : BindableObject
    {
        public Estudiante Estudiante { get; set; }

        public string Legajo => Estudiante?.Legajo ?? string.Empty;
        public string NombreCompleto => Estudiante?.NombreCompleto ?? string.Empty;
        public string Email => Estudiante?.Email ?? string.Empty;
        public string Dni => Estudiante?.Dni ?? string.Empty;
        public string Telefono => Estudiante?.Telefono ?? string.Empty;

        public double Promedio { get; set; }
        public int PorcentajeAsistencia { get; set; }
        public bool EnRiesgo => Promedio < 6.0 || PorcentajeAsistencia < 75;

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

        // --- Propiedades para el Estado de Contacto (H11) ---
        private bool _fueContactado;
        public bool FueContactado
        {
            get => _fueContactado;
            set { _fueContactado = value; OnPropertyChanged(); }
        }

        private string _textoEstadoContacto = "Sin contacto registrado";
        public string TextoEstadoContacto
        {
            get => _textoEstadoContacto;
            set { _textoEstadoContacto = value; OnPropertyChanged(); }
        }

        // --- Alias de compatibilidad para el XAML ---
        public string ColorHex => ColorRiesgoHex;
        public string Diagnostico => DiagnosticoRiesgo;
        public string NivelRiesgoText => NivelRiesgoEnum.ToString().ToUpper();

        // --- Constructores ---
        public EstudianteItemViewModel(Estudiante estudiante, double promedio, int porcentajeAsistencia)
        {
            Estudiante = estudiante;
            Promedio = promedio;
            PorcentajeAsistencia = porcentajeAsistencia;
        }

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

    public enum NivelRiesgo
    {
        Bajo,
        Medio,
        Alto
    }
}