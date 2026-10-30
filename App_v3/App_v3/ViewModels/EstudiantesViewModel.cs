using System.Collections.ObjectModel;
using System.Windows.Input;
using App_v3.Dominio;

namespace App_v3.ViewModels;

/// <summary>H1, H9 y H13: alta de estudiantes, nómina de la comisión, bajas y ficha.</summary>
public class EstudiantesViewModel : PaginaViewModel
{
    public const string TodosLosEstados = "Todos";
    public const string SoloActivos = "Activos";
    public const string SoloInactivos = "Inactivos";

    private readonly EstudianteService _estudiantes;
    private readonly InscripcionService _inscripciones;
    private readonly SeguimientoService _seguimiento;
    private readonly TimeProvider _reloj;

    private string _legajo = string.Empty;
    private string _dni = string.Empty;
    private string _nombre = string.Empty;
    private string _apellido = string.Empty;
    private string _email = string.Empty;
    private string _telefono = string.Empty;
    private string _dniExistente = string.Empty;
    private string _filtro = TodosLosEstados;
    private FichaEstudianteViewModel? _ficha;

    public EstudiantesViewModel(
        SelectorComisionViewModel selector,
        EstudianteService estudiantes,
        InscripcionService inscripciones,
        SeguimientoService seguimiento,
        TimeProvider reloj,
        FormularioContactoViewModel contacto,
        IDialogService dialogos) : base(selector, dialogos)
    {
        _estudiantes = estudiantes;
        _inscripciones = inscripciones;
        _seguimiento = seguimiento;
        _reloj = reloj;
        Contacto = contacto;
        Contacto.AlRegistrar = RecargarFichaAsync;
        GuardarCommand = Comando(GuardarAsync);
        InscribirExistenteCommand = Comando(InscribirExistenteAsync);
    }

    public ObservableCollection<FilaNominaViewModel> Nomina { get; } = [];
    public FormularioContactoViewModel Contacto { get; }
    public IReadOnlyList<string> Filtros { get; } = [TodosLosEstados, SoloActivos, SoloInactivos];

    public ICommand GuardarCommand { get; }
    public ICommand InscribirExistenteCommand { get; }

    public string Legajo { get => _legajo; set => SetProperty(ref _legajo, value); }
    public string Dni { get => _dni; set => SetProperty(ref _dni, value); }
    public string Nombre { get => _nombre; set => SetProperty(ref _nombre, value); }
    public string Apellido { get => _apellido; set => SetProperty(ref _apellido, value); }
    public string Email { get => _email; set => SetProperty(ref _email, value); }
    public string Telefono { get => _telefono; set => SetProperty(ref _telefono, value); }
    public string DniExistente { get => _dniExistente; set => SetProperty(ref _dniExistente, value); }

    public string Filtro
    {
        get => _filtro;
        set
        {
            if (SetProperty(ref _filtro, value)) _ = EjecutarAsync(CargarAsync);
        }
    }

    public FichaEstudianteViewModel? Ficha
    {
        get => _ficha;
        private set
        {
            if (!SetProperty(ref _ficha, value)) return;
            OnPropertyChanged(nameof(HayFicha));
        }
    }

    public bool HayFicha => Ficha is not null;

    public override async Task CargarAsync()
    {
        Nomina.Clear();
        Ficha = null;
        Contacto.Cerrar();
        if (Selector.ComisionId is not { } comisionId) return;

        var nomina = await _inscripciones.ListarNominaAsync(comisionId);
        foreach (var inscripto in nomina.Where(PasaElFiltro))
            Nomina.Add(new FilaNominaViewModel(
                inscripto, fila => EjecutarAsync(() => VerFichaAsync(fila)), fila => EjecutarAsync(() => CambiarEstadoAsync(fila))));
    }

    private bool PasaElFiltro(InscriptoEnComision inscripto) => Filtro switch
    {
        SoloActivos => inscripto.Activo,
        SoloInactivos => !inscripto.Activo,
        _ => true
    };

    // CA 1.1 y CA 1.2: el alta valida los datos y rechaza legajo o DNI repetidos; después inscribe en la comisión elegida.
    private async Task GuardarAsync()
    {
        var comisionId = ExigirComision();
        var datos = new DatosEstudiante(Legajo, Dni, Nombre, Apellido, Email, Telefono);

        var estudiante = await _estudiantes.AltaAsync(datos);
        await _inscripciones.InscribirAsync(estudiante.Id, comisionId);

        LimpiarFormulario();
        await CargarAsync();
        await Dialogos.AlertAsync("Estudiante guardado", $"{estudiante.NombreCompleto} quedó inscripto en la comisión {Selector.ComisionSeleccionada?.Nombre}.");
    }

    // Un estudiante ya cargado puede cursar otra comisión: se lo busca por DNI y se lo inscribe.
    private async Task InscribirExistenteAsync()
    {
        var comisionId = ExigirComision();
        var estudiante = await _estudiantes.ObtenerPorDniAsync(DniExistente)
            ?? throw new ReglaDeNegocioException("No hay ningún estudiante con ese DNI. Cárguelo con el formulario de alta.");

        await _inscripciones.InscribirAsync(estudiante.Id, comisionId);

        DniExistente = string.Empty;
        await CargarAsync();
        await Dialogos.AlertAsync("Estudiante inscripto", $"{estudiante.NombreCompleto} quedó inscripto en la comisión {Selector.ComisionSeleccionada?.Nombre}.");
    }

    // CA 9.1 a 9.3, CA 12.1 a 12.4 y CA 13.2
    private Task VerFichaAsync(FilaNominaViewModel fila) => MostrarFichaAsync(fila.Estudiante.Id);

    private async Task MostrarFichaAsync(int estudianteId)
    {
        var ficha = await _seguimiento.ObtenerFichaAsync(estudianteId, ExigirComision());
        Ficha = new FichaEstudianteViewModel(
            ficha, () => { Ficha = null; Contacto.Cerrar(); }, () => Contacto.Abrir(ficha.Estudiante));
    }

    // Después de registrar un contacto la ficha se muestra de nuevo con el contacto nuevo en su historial (CA 10.1).
    private async Task RecargarFichaAsync()
    {
        if (Ficha is { } actual) await MostrarFichaAsync(actual.Estudiante.Id);
    }

    // CA 1.4 y CA 13.3: la baja no borra nada, solo marca la inscripción como inactiva.
    private async Task CambiarEstadoAsync(FilaNominaViewModel fila)
    {
        var comisionId = ExigirComision();
        if (fila.Activo)
        {
            var acepta = await Dialogos.ConfirmarAsync(
                "Dar de baja", $"¿Dar de baja a {fila.Estudiante.NombreCompleto}? No se borra nada: queda inactivo y se conserva todo su historial.");
            if (!acepta) return;

            await _inscripciones.DarDeBajaAsync(fila.Estudiante.Id, comisionId, _reloj.GetLocalNow().DateTime);
        }
        else
        {
            await _inscripciones.ReactivarAsync(fila.Estudiante.Id, comisionId);
        }

        await CargarAsync();
    }

    private void LimpiarFormulario()
    {
        Legajo = Dni = Nombre = Apellido = Email = Telefono = string.Empty;
    }
}
