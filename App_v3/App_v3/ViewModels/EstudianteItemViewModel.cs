using System.Windows.Input;
using App_v3.Models;

namespace App_v3.ViewModels
{
    // Una fila del panel de seguimiento. Historias H8 (riesgo), H10 (contacto) y H11 (último contacto).
    public class EstudianteItemViewModel : BindableObject
    {
        public Estudiante Estudiante { get; }

        public string NombreCompleto => Estudiante.NombreCompleto;
        public string Legajo => Estudiante.Legajo;
        public string Email => Estudiante.Email;

        // Los dos datos que justifican el nivel de riesgo (criterio 8.2).
        public int Inasistencias => Estudiante.Inasistencias;
        public int TpsNoEntregados => Estudiante.TpsNoEntregados;

        public NivelRiesgo NivelRiesgoEnum { get; }
        public string ColorRiesgoHex { get; }
        public string DiagnosticoRiesgo { get; }

        public string NivelTexto => NivelRiesgoEnum switch
        {
            NivelRiesgo.Alto => "ALTO",
            NivelRiesgo.Medio => "MEDIO",
            _ => "BAJO"
        };

        // Fecha del último contacto, o null si todavía no se lo contactó (criterio 11.1).
        public DateTime? UltimoContacto { get; }

        public string TextoContacto => UltimoContacto is { } fecha
            ? $"Contactado el {fecha:dd/MM}"
            : "Sin contacto registrado";

        // Abre el formulario de contacto para este estudiante.
        public ICommand ContactarCommand { get; }

        public EstudianteItemViewModel(
            Estudiante estudiante,
            NivelRiesgo nivelRiesgo,
            string colorHex,
            string diagnostico,
            DateTime? ultimoContacto,
            Action<EstudianteItemViewModel> alContactar)
        {
            Estudiante = estudiante;
            NivelRiesgoEnum = nivelRiesgo;
            ColorRiesgoHex = colorHex;
            DiagnosticoRiesgo = diagnostico;
            UltimoContacto = ultimoContacto;
            ContactarCommand = new Command(() => alContactar(this));
        }
    }
}
