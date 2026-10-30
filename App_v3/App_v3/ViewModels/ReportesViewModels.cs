using System.Collections.ObjectModel;
using System.Windows.Input;
using App_v3.Dominio;

namespace App_v3.ViewModels;

/// <summary>Menú de reportes: lleva a cada uno de los dos reportes.</summary>
public class MenuReportesViewModel(INavegacionService navegacion, IDialogService dialogos) : BaseViewModel(dialogos)
{
    public ICommand IrAsistenciasCommand { get; } = new ComandoAsync(() => navegacion.IrAAsync(Rutas.ReporteAsistencias));
    public ICommand IrEntregasCommand { get; } = new ComandoAsync(() => navegacion.IrAAsync(Rutas.ReporteEntregas));
}

/// <summary>Un renglón de un reporte: un texto principal, un detalle y el estado con su color.</summary>
public record RenglonDeReporte(string Titulo, string Detalle, string TextoEstado, object Estado);

/// <summary>Reporte de asistencias de la comisión, filtrable por DNI y por fechas.</summary>
public class ReporteAsistenciasViewModel : PaginaViewModel
{
    private readonly ReporteService _reportes;
    private readonly INavegacionService _navegacion;
    private readonly TimeProvider _reloj;
    private string _dni = string.Empty;
    private DateTime _desde;
    private DateTime _hasta;

    public ReporteAsistenciasViewModel(
        SelectorComisionViewModel selector, ReporteService reportes, INavegacionService navegacion, TimeProvider reloj, IDialogService dialogos)
        : base(selector, dialogos)
    {
        _reportes = reportes;
        _navegacion = navegacion;
        _reloj = reloj;
        RestablecerFechas();
        BuscarCommand = Comando(CargarAsync);
        LimpiarCommand = Comando(LimpiarAsync);
        VolverCommand = Comando(_navegacion.VolverAsync);
    }

    public ObservableCollection<RenglonDeReporte> Filas { get; } = [];
    public ICommand BuscarCommand { get; }
    public ICommand LimpiarCommand { get; }
    public ICommand VolverCommand { get; }

    public string Dni { get => _dni; set => SetProperty(ref _dni, value); }
    public DateTime Desde { get => _desde; set => SetProperty(ref _desde, value); }
    public DateTime Hasta { get => _hasta; set => SetProperty(ref _hasta, value); }

    public override async Task CargarAsync()
    {
        Filas.Clear();
        if (Selector.ComisionId is not { } comisionId) return;

        foreach (var fila in await _reportes.AsistenciasAsync(comisionId, Dni, Desde, Hasta))
            Filas.Add(new RenglonDeReporte(
                fila.Estudiante.NombreCompleto, $"DNI {fila.Estudiante.Dni} · {Textos.De(fila.Clase)}", Textos.De(fila.Estado), fila.Estado));
    }

    private async Task LimpiarAsync()
    {
        Dni = string.Empty;
        RestablecerFechas();
        await CargarAsync();
    }

    private void RestablecerFechas()
    {
        Hasta = _reloj.GetLocalNow().DateTime.Date;
        Desde = Hasta.AddMonths(-3);
    }
}

/// <summary>Reporte de entregas de la comisión, filtrable por DNI y por trabajo práctico.</summary>
public class ReporteEntregasViewModel : PaginaViewModel
{
    private readonly ReporteService _reportes;
    private readonly CalendarioService _calendario;
    private readonly INavegacionService _navegacion;
    private string _dni = string.Empty;
    private TrabajoOpcion _trabajo = TodosLosTrabajos;

    private static readonly TrabajoOpcion TodosLosTrabajos = new(null, "Todos los trabajos prácticos");

    public ReporteEntregasViewModel(
        SelectorComisionViewModel selector, ReporteService reportes, CalendarioService calendario, INavegacionService navegacion, IDialogService dialogos)
        : base(selector, dialogos)
    {
        _reportes = reportes;
        _calendario = calendario;
        _navegacion = navegacion;
        BuscarCommand = Comando(CargarFilasAsync);
        LimpiarCommand = Comando(LimpiarAsync);
        VolverCommand = Comando(_navegacion.VolverAsync);
    }

    public ObservableCollection<TrabajoOpcion> Trabajos { get; } = [];
    public ObservableCollection<RenglonDeReporte> Filas { get; } = [];
    public ICommand BuscarCommand { get; }
    public ICommand LimpiarCommand { get; }
    public ICommand VolverCommand { get; }

    public string Dni { get => _dni; set => SetProperty(ref _dni, value); }
    public TrabajoOpcion Trabajo { get => _trabajo; set => SetProperty(ref _trabajo, value ?? TodosLosTrabajos); }

    public override async Task CargarAsync()
    {
        var elegido = Trabajo.Trabajo?.Id;
        Trabajos.Clear();
        Trabajos.Add(TodosLosTrabajos);
        if (Selector.ComisionId is { } comisionId)
            foreach (var trabajo in await _calendario.ListarTrabajosAsync(comisionId))
                Trabajos.Add(TrabajoOpcion.De(trabajo));

        _trabajo = Trabajos.FirstOrDefault(t => t.Trabajo?.Id == elegido) ?? TodosLosTrabajos;
        OnPropertyChanged(nameof(Trabajo));
        await CargarFilasAsync();
    }

    private async Task CargarFilasAsync()
    {
        Filas.Clear();
        if (Selector.ComisionId is not { } comisionId) return;

        foreach (var fila in await _reportes.EntregasAsync(comisionId, Dni, Trabajo.Trabajo?.Id))
            Filas.Add(new RenglonDeReporte(
                fila.Estudiante.NombreCompleto, $"DNI {fila.Estudiante.Dni} · {Textos.De(fila.Trabajo)}", Textos.De(fila.Estado), fila.Estado));
    }

    private async Task LimpiarAsync()
    {
        Dni = string.Empty;
        _trabajo = TodosLosTrabajos;
        OnPropertyChanged(nameof(Trabajo));
        await CargarFilasAsync();
    }
}
