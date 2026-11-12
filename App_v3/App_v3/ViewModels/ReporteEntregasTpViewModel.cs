// App_v3/App_v3/ViewModels/ReporteEntregasTpViewModel.cs

using System.Collections.ObjectModel;
using System.Windows.Input;
using App_v3.Models;
using App_v3.Services;

namespace App_v3.ViewModels;

public class ReporteEntregasTpViewModel : BindableObject
{
    private readonly DatabaseService _databaseService;

    // Propiedad renombrada para coincidir exactamente con el XAML (ListaTrabajosPracticos)
    public ObservableCollection<TrabajoPractico> ListaTrabajosPracticos { get; set; } = new();

    // Propiedad renombrada para coincidir exactamente con el XAML (ListaReporteTps)
    public ObservableCollection<ReporteTpItem> ListaReporteTps { get; set; } = new();

    private TrabajoPractico? _tpSeleccionado;
    public TrabajoPractico? TpSeleccionado
    {
        get => _tpSeleccionado;
        set
        {
            if (_tpSeleccionado != value)
            {
                _tpSeleccionado = value;
                OnPropertyChanged();
                _ = CargarReporteAsync(); // Filtra automáticamente al cambiar el TP
            }
        }
    }

    private string _dniFiltro = string.Empty;
    public string DniFiltro
    {
        get => _dniFiltro;
        set
        {
            if (_dniFiltro != value)
            {
                _dniFiltro = value;
                OnPropertyChanged();
            }
        }
    }

    public ICommand AplicarFiltrosCommand { get; }
    public ICommand LimpiarFiltrosCommand { get; }
    public ICommand VolverAlMenuCommand { get; }

    public ReporteEntregasTpViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;

        // Coincide con el comando del botón en el XAML
        AplicarFiltrosCommand = new Command(async () => await CargarReporteAsync());
        LimpiarFiltrosCommand = new Command(async () => await LimpiarFiltrosAsync());
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
        try
        {
            var tps = await _databaseService.GetTrabajosPracticosAsync();

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                ListaTrabajosPracticos.Clear();
                // Opción comodín para ver todos los TPs a la vez
                ListaTrabajosPracticos.Add(new TrabajoPractico { Id = 0, Nombre = "Todos los Trabajos Prácticos" });

                if (tps != null)
                {
                    foreach (var tp in tps)
                    {
                        ListaTrabajosPracticos.Add(tp);
                    }
                }

                if (ListaTrabajosPracticos.Count > 0 && TpSeleccionado == null)
                {
                    TpSeleccionado = ListaTrabajosPracticos.First();
                }
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Error al cargar TPs]: {ex.Message}");
        }
    }

    public async Task CargarReporteAsync()
    {
        try
        {
            int? tpId = (TpSeleccionado != null && TpSeleccionado.Id > 0) ? TpSeleccionado.Id : (int?)null;

            var resultados = await _databaseService.ObtenerReporteEntregasTpAsync(DniFiltro, tpId);

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                ListaReporteTps.Clear();
                if (resultados != null)
                {
                    foreach (var item in resultados)
                    {
                        ListaReporteTps.Add(item);
                    }
                }
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Error al cargar reporte de TPs]: {ex.Message}");
        }
    }

    private async Task LimpiarFiltrosAsync()
    {
        DniFiltro = string.Empty;
        if (ListaTrabajosPracticos.Count > 0)
        {
            TpSeleccionado = ListaTrabajosPracticos.First();
        }
        await CargarReporteAsync();
    }
}