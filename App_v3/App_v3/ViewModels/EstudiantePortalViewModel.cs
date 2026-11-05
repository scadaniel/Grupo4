using System.Collections.ObjectModel;
using System.Windows.Input;
using App_v3.Dominio;

namespace App_v3.ViewModels;

/// <summary>
/// H14: Portal del Estudiante. Permite consultar el rendimiento, inasistencias,
/// estado de riesgo y entregas de manera individual.
/// </summary>
public class EstudiantePortalViewModel : BaseViewModel
{
    private readonly EstudianteService _estudiantes;
    private readonly InscripcionService _inscripciones;
    private readonly SeguimientoService _seguimiento;
    private readonly IRepositorio _repositorio;
    private string _documentoBusqueda = string.Empty;
    private string _mensajeError = string.Empty;
    private Estudiante? _estudianteActual;
    private FichaEstudianteViewModel? _ficha;

    public EstudiantePortalViewModel(
        EstudianteService estudiantes,
        InscripcionService inscripciones,
        SeguimientoService seguimiento,
        IRepositorio repositorio,
        IDialogService dialogos) : base(dialogos)
    {
        _estudiantes = estudiantes;
        _inscripciones = inscripciones;
        _seguimiento = seguimiento;
        _repositorio = repositorio;

        BuscarCommand = Comando(BuscarEstudianteAsync);
        CambiarEstudianteCommand = Comando(() => { EstudianteActual = null; Ficha = null; DocumentoBusqueda = string.Empty; MensajeError = string.Empty; return Task.CompletedTask; });
    }

    public ICommand BuscarCommand { get; }
    public ICommand CambiarEstudianteCommand { get; }

    public string DocumentoBusqueda
    {
        get => _documentoBusqueda;
        set => SetProperty(ref _documentoBusqueda, value);
    }

    public string MensajeError
    {
        get => _mensajeError;
        private set => SetProperty(ref _mensajeError, value);
    }

    public Estudiante? EstudianteActual
    {
        get => _estudianteActual;
        private set
        {
            if (SetProperty(ref _estudianteActual, value))
            {
                OnPropertyChanged(nameof(TieneEstudiante));
                OnPropertyChanged(nameof(NoTieneEstudiante));
            }
        }
    }

    public bool TieneEstudiante => EstudianteActual is not null;
    public bool NoTieneEstudiante => EstudianteActual is null;

    public FichaEstudianteViewModel? Ficha
    {
        get => _ficha;
        private set => SetProperty(ref _ficha, value);
    }

    private async Task BuscarEstudianteAsync()
    {
        MensajeError = string.Empty;

        if (string.IsNullOrWhiteSpace(DocumentoBusqueda))
        {
            MensajeError = "Complete el campo de DNI o Legajo.";
            return;
        }

        var busquedaLimpia = DocumentoBusqueda.Replace(".", string.Empty).Replace(" ", string.Empty).Trim();

        // Validación de longitud errónea (por ejemplo, menor a 3 caracteres o DNI incompleto)
        if (busquedaLimpia.Length < 3)
        {
            MensajeError = "Longitud de documento o legajo errónea, vuelva a intentarlo.";
            return;
        }

        var todos = await _estudiantes.ListarAsync();
        
        var encontrado = todos.FirstOrDefault(e => 
            e.Dni == busquedaLimpia || 
            string.Equals(e.Legajo, busquedaLimpia, StringComparison.OrdinalIgnoreCase));

        if (encontrado is null)
        {
            MensajeError = "Legajo o numero de DNI erróneos, vuelva a intentarlo.";
            return;
        }

        EstudianteActual = encontrado;

        var todasInscripciones = await _repositorio.ListarAsync<Inscripcion>();
        var inscripcionActiva = todasInscripciones.FirstOrDefault(i => i.EstudianteId == encontrado.Id);

        if (inscripcionActiva is not null)
        {
            var fichaDominio = await _seguimiento.ObtenerFichaAsync(encontrado.Id, inscripcionActiva.ComisionId);
            Ficha = new FichaEstudianteViewModel(fichaDominio, () => { }, () => { });
        }
        else
        {
            MensajeError = "El estudiante no se encuentra inscripto en ninguna comisión activa.";
            EstudianteActual = null;
        }
    }
}