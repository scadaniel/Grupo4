using System.Collections.ObjectModel;
using System.Windows.Input;
using App_v3.Dominio;

namespace App_v3.ViewModels;

/// <summary>Una clase del calendario tal como se muestra en el selector.</summary>
public record ClaseOpcion(ClaseDictada Clase)
{
    public string Texto => Textos.De(Clase);
}

/// <summary>H3 y H4: tomar la asistencia de una clase y corregirla.</summary>
public class AsistenciaViewModel : PaginaViewModel
{
    private readonly AsistenciaService _asistencias;
    private readonly CalendarioService _calendario;
    private ClaseOpcion? _claseSeleccionada;
    private bool _yaRegistrada;
    private bool _editando;
    private bool _recargando;

    public AsistenciaViewModel(
        SelectorComisionViewModel selector, AsistenciaService asistencias, CalendarioService calendario, IDialogService dialogos)
        : base(selector, dialogos)
    {
        _asistencias = asistencias;
        _calendario = calendario;
        GuardarCommand = Comando(GuardarAsync);
        ModificarCommand = Comando(ModificarAsync);
        GuardarCambiosCommand = Comando(GuardarCambiosAsync);
    }

    public ObservableCollection<ClaseOpcion> Clases { get; } = [];
    public ObservableCollection<LineaAsistenciaViewModel> Lineas { get; } = [];

    public ICommand GuardarCommand { get; }
    public ICommand ModificarCommand { get; }
    public ICommand GuardarCambiosCommand { get; }

    public ClaseOpcion? ClaseSeleccionada
    {
        get => _claseSeleccionada;
        set
        {
            if (SetProperty(ref _claseSeleccionada, value) && !_recargando) _ = EjecutarAsync(CargarPlanillaAsync);
        }
    }

    public bool HayClase => ClaseSeleccionada is not null && Lineas.Count > 0;

    // CA 3.4: avisa que la clase ya tiene un registro.
    public bool YaRegistrada
    {
        get => _yaRegistrada;
        private set
        {
            if (SetProperty(ref _yaRegistrada, value)) NotificarBotones();
        }
    }

    public bool Editando
    {
        get => _editando;
        private set
        {
            if (!SetProperty(ref _editando, value)) return;
            NotificarBotones();
            foreach (var linea in Lineas) linea.Editable = !YaRegistrada || value;
        }
    }

    public string Aviso => !YaRegistrada
        ? string.Empty
        : Editando
            ? "Está modificando una asistencia ya guardada."
            : "Ya existe un registro de asistencia para esta clase. Toque Modificar asistencia para corregirlo.";

    public bool MostrarGuardar => HayClase && !YaRegistrada;
    public bool MostrarModificar => HayClase && YaRegistrada && !Editando;
    public bool MostrarGuardarCambios => HayClase && YaRegistrada && Editando;

    public override async Task CargarAsync()
    {
        var elegida = ClaseSeleccionada?.Clase.Id;
        var clases = Selector.ComisionId is { } comisionId
            ? await _calendario.ListarClasesAsync(comisionId)
            : [];

        // El selector de la pantalla pone la selección en null al vaciar la lista: no debe disparar otra carga.
        _recargando = true;
        try
        {
            Clases.Clear();
            foreach (var clase in clases) Clases.Add(new ClaseOpcion(clase));
        }
        finally
        {
            _recargando = false;
        }

        _claseSeleccionada = Clases.FirstOrDefault(c => c.Clase.Id == elegida);
        OnPropertyChanged(nameof(ClaseSeleccionada));
        await CargarPlanillaAsync();
    }

    // CA 3.1 y CA 4.1: todos presentes si es nueva; los estados guardados si ya tiene registro.
    private async Task CargarPlanillaAsync()
    {
        Lineas.Clear();
        _editando = false;
        _yaRegistrada = false;

        if (ClaseSeleccionada is not null)
        {
            var planilla = await _asistencias.ObtenerPlanillaAsync(ClaseSeleccionada.Clase.Id);
            _yaRegistrada = planilla.YaRegistrada;
            foreach (var linea in planilla.Lineas)
                Lineas.Add(new LineaAsistenciaViewModel(linea, editable: !planilla.YaRegistrada));
        }

        NotificarBotones();
        OnPropertyChanged(nameof(YaRegistrada));
        OnPropertyChanged(nameof(Editando));
    }

    // CA 3.3
    private async Task GuardarAsync()
    {
        await _asistencias.GuardarAsync(ClaseExigida(), Lineas.Select(l => l.ALinea()));
        await CargarPlanillaAsync();
        await Dialogos.AlertAsync("Asistencia guardada", "Se guardó la asistencia de la clase y se actualizó el riesgo de los estudiantes.");
    }

    // CA 4.2
    private Task ModificarAsync()
    {
        Editando = true;
        return Task.CompletedTask;
    }

    // CA 4.3 y CA 4.4
    private async Task GuardarCambiosAsync()
    {
        await _asistencias.CorregirAsync(ClaseExigida(), Lineas.Select(l => l.ALinea()));
        await CargarPlanillaAsync();
        await Dialogos.AlertAsync("Asistencia modificada", "La asistencia fue actualizada correctamente.");
    }

    private int ClaseExigida() =>
        ClaseSeleccionada?.Clase.Id ?? throw new ReglaDeNegocioException("Elija una clase del calendario.");

    private void NotificarBotones()
    {
        OnPropertyChanged(nameof(HayClase));
        OnPropertyChanged(nameof(Aviso));
        OnPropertyChanged(nameof(MostrarGuardar));
        OnPropertyChanged(nameof(MostrarModificar));
        OnPropertyChanged(nameof(MostrarGuardarCambios));
    }
}
