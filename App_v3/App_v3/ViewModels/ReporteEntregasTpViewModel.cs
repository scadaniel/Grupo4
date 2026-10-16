using System.Collections.ObjectModel;
using System.Windows.Input;
using App_v3.Models;
using App_v3.Services;
using App_v3.Views;

namespace App_v3.ViewModels;

public class ReporteEntregasTpViewModel : BindableObject
{
    private readonly DatabaseService _databaseService;

    public ObservableCollection<TrabajoPractico> TrabajosPracticos { get; set; } = new();
    public ObservableCollection<ReporteTpItem> ListaReporteTp { get; set; } = new();

    private TrabajoPractico? _tpSeleccionado;
    public TrabajoPractico? TpSeleccionado
    {
        get => _tpSeleccionado;
        set { _tpSeleccionado = value; OnPropertyChanged(); }
    }

    private string _dniFiltro = string.Empty;
    public string DniFiltro
    {
        get => _dniFiltro;
        set { _dniFiltro = value; OnPropertyChanged(); }
    }

    public ICommand BuscarCommand { get; }
    public ICommand LimpiarFiltrosCommand { get; }
    public ICommand VolverAlMenuCommand { get; }

    public ReporteEntregasTpViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
        BuscarCommand = new Command(async () => await CargarReporteAsync());
        LimpiarFiltrosCommand = new Command(async () => await LimpiarFiltrosAsync());

        // Inicializamos el comando de retorno absoluto al menú de reportes
        VolverAlMenuCommand = new Command(async () => await Shell.Current.GoToAsync(".."));

        _ = InicializarAsync();
    }

    private async Task InicializarAsync()
    {
        await CargarTrabajosPracticosAsync();
        await CargarReporteAsync();
    }

    private async Task CargarTrabajosPracticosAsync()
    {
        var tps = await _databaseService.GetTrabajosPracticosAsync();
        TrabajosPracticos.Clear();

        // Opción comodín para ver todos los TPs a la vez
        TrabajosPracticos.Add(new TrabajoPractico { Id = 0, Nombre = "Todos los Trabajos Prácticos" });

        foreach (var tp in tps)
        {
            TrabajosPracticos.Add(tp);
        }
        TpSeleccionado = TrabajosPracticos.First();
    }

    public async Task CargarReporteAsync()

    {
        int? tpId = (TpSeleccionado != null && TpSeleccionado.Id > 0) ? TpSeleccionado.Id : (int?)null;

        var resultados = await _databaseService.ObtenerReporteEntregasTpAsync(DniFiltro, tpId);

        ListaReporteTp.Clear();
        foreach (var item in resultados)
        {
            ListaReporteTp.Add(item);
        }
    }

    private async Task LimpiarFiltrosAsync()
    {
        DniFiltro = string.Empty;
        if (TrabajosPracticos.Count > 0) TpSeleccionado = TrabajosPracticos.First();
        await CargarReporteAsync();
    }
}