using System.Collections.ObjectModel;
using System.Windows.Input;
using App_v3.Dominio;

namespace App_v3.ViewModels;

/// <summary>Un trabajo práctico tal como se muestra en el selector.</summary>
public record TrabajoOpcion(TrabajoPractico? Trabajo, string Texto)
{
    public static TrabajoOpcion De(TrabajoPractico trabajo) => new(trabajo, Textos.De(trabajo));
}

/// <summary>Un estudiante en la planilla de entregas.</summary>
public class LineaEntregaViewModel(LineaEntrega linea) : NotificadorBase
{
    private bool _entregado = linea.Entregado;

    public Estudiante Estudiante { get; } = linea.Estudiante;
    public string NombreCompleto => $"{Estudiante.Apellido}, {Estudiante.Nombre}";
    public string Legajo => $"Legajo {Estudiante.Legajo}";

    public bool Entregado
    {
        get => _entregado;
        set
        {
            if (SetProperty(ref _entregado, value)) OnPropertyChanged(nameof(TextoEstado));
        }
    }

    public string TextoEstado => Entregado ? "Entregó" : "No entregó";

    public LineaEntrega ALinea() => new(Estudiante, Entregado);
}

/// <summary>H5: registrar quién entregó y quién no entregó cada trabajo práctico.</summary>
public class EntregasViewModel : PaginaViewModel
{
    private readonly EntregaService _entregas;
    private readonly CalendarioService _calendario;
    private TrabajoOpcion? _trabajoSeleccionado;
    private string _aviso = string.Empty;
    private bool _recargando;

    public EntregasViewModel(
        SelectorComisionViewModel selector, EntregaService entregas, CalendarioService calendario, IDialogService dialogos)
        : base(selector, dialogos)
    {
        _entregas = entregas;
        _calendario = calendario;
        GuardarCommand = Comando(GuardarAsync);
    }

    public ObservableCollection<TrabajoOpcion> Trabajos { get; } = [];
    public ObservableCollection<LineaEntregaViewModel> Lineas { get; } = [];
    public ICommand GuardarCommand { get; }

    public TrabajoOpcion? TrabajoSeleccionado
    {
        get => _trabajoSeleccionado;
        set
        {
            if (SetProperty(ref _trabajoSeleccionado, value) && !_recargando) _ = EjecutarAsync(CargarPlanillaAsync);
        }
    }

    public bool HayPlanilla => Lineas.Count > 0;
    public bool SinTrabajos => Selector.HayComision && Trabajos.Count == 0;

    // CA 5.4: si el trabajo ya tiene información, se avisa y se muestra lo guardado.
    public string Aviso
    {
        get => _aviso;
        private set => SetProperty(ref _aviso, value);
    }

    public override async Task CargarAsync()
    {
        var elegido = TrabajoSeleccionado?.Trabajo?.Id;
        var trabajos = Selector.ComisionId is { } comisionId
            ? await _calendario.ListarTrabajosAsync(comisionId)
            : [];

        // El selector de la pantalla pone la selección en null al vaciar la lista: no debe disparar otra carga.
        _recargando = true;
        try
        {
            Trabajos.Clear();
            foreach (var trabajo in trabajos) Trabajos.Add(TrabajoOpcion.De(trabajo));
        }
        finally
        {
            _recargando = false;
        }

        OnPropertyChanged(nameof(SinTrabajos));
        _trabajoSeleccionado = Trabajos.FirstOrDefault(t => t.Trabajo?.Id == elegido);
        OnPropertyChanged(nameof(TrabajoSeleccionado));
        await CargarPlanillaAsync();
    }

    // CA 5.3 y CA 5.4
    private async Task CargarPlanillaAsync()
    {
        Lineas.Clear();
        Aviso = string.Empty;

        if (TrabajoSeleccionado?.Trabajo is { } trabajo)
        {
            var planilla = await _entregas.ObtenerPlanillaAsync(trabajo.Id);
            foreach (var linea in planilla.Lineas) Lineas.Add(new LineaEntregaViewModel(linea));
            Aviso = planilla.YaRegistrada ? "Este trabajo ya tiene entregas registradas. Se muestra lo guardado." : string.Empty;
        }

        OnPropertyChanged(nameof(HayPlanilla));
    }

    // CA 5.1 y CA 5.2
    private async Task GuardarAsync()
    {
        var trabajo = TrabajoSeleccionado?.Trabajo
            ?? throw new ReglaDeNegocioException("Elija un trabajo práctico del calendario.");

        await _entregas.GuardarAsync(trabajo.Id, Lineas.Select(l => l.ALinea()));
        await CargarPlanillaAsync();
        await Dialogos.AlertAsync("Entregas guardadas", "Se registraron las entregas y se actualizó el riesgo de los estudiantes.");
    }
}