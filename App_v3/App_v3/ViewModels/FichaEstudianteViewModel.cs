using System.Windows.Input;
using App_v3.Dominio;

namespace App_v3.ViewModels;

/// <summary>Un renglón de las listas de la ficha: un texto, su estado y el color que le corresponde.</summary>
public record RenglonDeFicha(string Texto, string TextoEstado, object Estado);

/// <summary>H9, H12 y H13: ficha, historial y resumen de un estudiante.</summary>
public class FichaEstudianteViewModel
{
    private readonly FichaEstudiante _ficha;

    public FichaEstudianteViewModel(FichaEstudiante ficha, Action cerrar, Action contactar)
    {
        _ficha = ficha;
        CerrarCommand = new ComandoAsync(() =>
        {
            cerrar();
            return Task.CompletedTask;
        });

        ContactarCommand = new ComandoAsync(() =>
        {
            contactar();
            return Task.CompletedTask;
        });

        Asistencias = ficha.Asistencias
            .Select(a => new RenglonDeFicha(Textos.De(a.Clase), Textos.De(a.Estado), a.Estado)).ToList();
        Entregas = ficha.Entregas
            .Select(e => new RenglonDeFicha(Textos.De(e.Trabajo), Textos.De(e.Estado), e.Estado)).ToList();
        Contactos = ficha.Contactos
            .Select(c => $"{Textos.Fecha(c.Fecha)} · {Textos.De(c.Medio)} · {c.Resultado}: {c.Nota}").ToList();
    }

    // CA 9.1
    public string NombreCompleto => _ficha.Estudiante.NombreCompleto;
    public string Legajo => _ficha.Estudiante.Legajo;
    public string Dni => _ficha.Estudiante.Dni;
    public string Email => _ficha.Estudiante.Email;
    public string Telefono => string.IsNullOrWhiteSpace(_ficha.Estudiante.Telefono) ? "No cargado" : _ficha.Estudiante.Telefono;

    // CA 9.3
    public bool EsActivo => _ficha.Activo;
    public string TextoEstado => _ficha.Activo ? "Activo" : $"Inactivo desde {Textos.Fecha(_ficha.FechaBaja ?? DateTime.Today)}";

    // CA 9.2: un inactivo ya no está en el panel, así que no tiene nivel de riesgo.
    public bool TieneNivel => _ficha.Nivel is not null;
    public NivelRiesgo Nivel => _ficha.Nivel ?? NivelRiesgo.Bajo;
    public string TextoNivel => _ficha.Nivel is { } nivel ? Textos.De(nivel) : "—";
    public string Motivo => _ficha.Motivo;

    // CA 12.3
    public string Resumen =>
        $"Inasistencias: {_ficha.TotalInasistencias} · TPs no entregados: {_ficha.TotalTpsNoEntregados}";

    // CA 12.1, CA 12.2 y CA 10.1
    public IReadOnlyList<RenglonDeFicha> Asistencias { get; }
    public IReadOnlyList<RenglonDeFicha> Entregas { get; }
    public IReadOnlyList<string> Contactos { get; }

    public ICommand CerrarCommand { get; }

    // CA 10.1: el contacto también se puede registrar desde la ficha. A un estudiante dado de baja ya no se lo contacta.
    public ICommand ContactarCommand { get; }
    public bool PuedeContactar => _ficha.Activo;
    public Estudiante Estudiante => _ficha.Estudiante;
}
