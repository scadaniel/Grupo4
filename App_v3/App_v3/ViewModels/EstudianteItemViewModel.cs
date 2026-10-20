using App_v3.Models;
using App_v3.Services;

namespace App_v3.ViewModels
{
    public class EstudianteItemViewModel : BindableObject
    {
        // Se mantiene el nombre "Estudiante" para no tocar el XAML
        public Persona Estudiante { get; }
        public double Promedio { get; }
        public int PorcentajeAsistencia { get; }

        public bool EnRiesgo => RiesgoAcademico.EnRiesgo(Promedio, PorcentajeAsistencia);

        // Propiedades directas para enlaces y ordenamientos
        public string Legajo => Estudiante?.Legajo ?? string.Empty;
        public string NombreCompleto => Estudiante?.NombreCompleto ?? string.Empty;
        public string Email => Estudiante?.Email ?? string.Empty;
        public string Telefono => Estudiante?.Telefono ?? string.Empty;

        // --- Propiedades para el motor de riesgo (H6, H8) ---
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

        public EstudianteItemViewModel(Persona estudiante, double promedio, int porcentajeAsistencia)
        {
            Estudiante = estudiante;
            Promedio = promedio;
            PorcentajeAsistencia = porcentajeAsistencia;
        }

        // Constructor para cuando el riesgo viene calculado (H8)
        public EstudianteItemViewModel(Persona estudiante, NivelRiesgo nivelRiesgo, string colorHex, string diagnostico)
        {
            Estudiante = estudiante;
            _nivelRiesgoEnum = nivelRiesgo;
            _colorRiesgoHex = colorHex;
            _diagnosticoRiesgo = diagnostico;

            Promedio = 7.0;
            PorcentajeAsistencia = 85;
        }
    }
}