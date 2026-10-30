using System.Windows.Input;
using App_v3.Dominio;

namespace App_v3.ViewModels;

/// <summary>Un estudiante en la nómina de la comisión (H1 CA 1.3, H13 CA 13.1).</summary>
public class FilaNominaViewModel
{
    public FilaNominaViewModel(
        InscriptoEnComision inscripto, Func<FilaNominaViewModel, Task> verFicha, Func<FilaNominaViewModel, Task> cambiarEstado)
    {
        Inscripto = inscripto;
        VerFichaCommand = new ComandoAsync(() => verFicha(this));
        CambiarEstadoCommand = new ComandoAsync(() => cambiarEstado(this));
    }

    public InscriptoEnComision Inscripto { get; }
    public Estudiante Estudiante => Inscripto.Estudiante;
    public bool Activo => Inscripto.Activo;

    public string NombreCompleto => $"{Estudiante.Apellido}, {Estudiante.Nombre}";
    public string Datos => $"Legajo {Estudiante.Legajo} · DNI {Estudiante.Dni}";
    public string Contacto => string.IsNullOrWhiteSpace(Estudiante.Telefono)
        ? Estudiante.Email
        : $"{Estudiante.Email} · {Estudiante.Telefono}";

    public string TextoEstado => Activo
        ? "Activo"
        : $"Inactivo desde {Textos.Fecha(Inscripto.FechaBaja ?? DateTime.Today)}";

    public string TextoAccion => Activo ? "Dar de baja" : "Reactivar";

    public ICommand VerFichaCommand { get; }
    public ICommand CambiarEstadoCommand { get; }
}
