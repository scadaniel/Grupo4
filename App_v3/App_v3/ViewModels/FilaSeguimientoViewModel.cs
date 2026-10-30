using System.Windows.Input;
using App_v3.Dominio;

namespace App_v3.ViewModels;

/// <summary>Una fila del panel de seguimiento: el estudiante, su nivel de riesgo y lo que lo justifica (H8, H10, H11).</summary>
public class FilaSeguimientoViewModel
{
    public FilaSeguimientoViewModel(SituacionEstudiante situacion, Func<FilaSeguimientoViewModel, Task> contactar)
    {
        Situacion = situacion;
        ContactarCommand = new ComandoAsync(() => contactar(this));
    }

    public SituacionEstudiante Situacion { get; }
    public Estudiante Estudiante => Situacion.Estudiante;

    public string NombreCompleto => $"{Estudiante.Apellido}, {Estudiante.Nombre}";
    public string Legajo => $"Legajo {Estudiante.Legajo}";

    // CA 8.3: el nivel se muestra con una etiqueta y un color.
    public NivelRiesgo Nivel => Situacion.Nivel;
    public string TextoNivel => Textos.De(Situacion.Nivel);

    // CA 8.2: los datos que justifican el nivel.
    public string Inasistencias => $"Inasistencias: {Situacion.Inasistencias}";
    public string TpsVencidos => $"TPs vencidos sin entregar: {Situacion.TpsVencidosNoEntregados}";
    public string Motivo => Situacion.Motivo;

    // CA 11.1
    public bool Contactado => Situacion.UltimoContacto is not null;
    public string TextoContacto => Situacion.UltimoContacto is { } fecha
        ? $"✔ Contactado el {fecha:dd/MM}"
        : "Sin contacto registrado";

    public ICommand ContactarCommand { get; }
}
