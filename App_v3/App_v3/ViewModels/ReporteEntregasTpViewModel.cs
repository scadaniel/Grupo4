using System.Collections.ObjectModel;
using System.Windows.Input;
using App_v3.Models;
using App_v3.Services;

namespace App_v3.ViewModels;

public class ReporteEntregasTpViewModel : BindableObject
{
    private readonly DatabaseService _databaseService;

    // Colecciones para los Pickers y el Listado
    public ObservableCollection<Comision> ListaComisiones { get; set; } = new();
    public ObservableCollection<TrabajoPractico> ListaTrabajosPracticos { get; set; } = new();
    public ObservableCollection<ReporteTpItem> ListaReporteTps { get; set; } = new();

    private Comision? _comisionSeleccionada;
    public Comision? ComisionSeleccionada
    {
        get => _comisionSeleccionada;
        set
        {
            if (_comisionSeleccionada != value)
            {
                _comisionSeleccionada = value;
                OnPropertyChanged();
                _ = CargarReporteAsync(); // Filtra automáticamente al cambiar la comisión
            }
        }
    }

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

        AplicarFiltrosCommand = new Command(async () => await CargarReporteAsync());
        LimpiarFiltrosCommand = new Command(async () => await LimpiarFiltrosAsync());
        VolverAlMenuCommand = new Command(async () => await Shell.Current.GoToAsync(".."));

        _ = InicializarAsync();
    }

    private async Task InicializarAsync()
    {
        await CargarComisionesAsync();
        await CargarTrabajosPracticosAsync();
        await CargarReporteAsync();
    }

    private async Task CargarComisionesAsync()
    {
        try
        {
            var comisiones = await _databaseService.GetComisionesAsync();

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                ListaComisiones.Clear();
                // Opción comodín para ver todas las comisiones
                ListaComisiones.Add(new Comision { Id = 0, Nombre = "Todas las Comisiones" });

                if (comisiones != null)
                {
                    var comisionesUnicas = comisiones
                        .Where(c => c != null && !string.IsNullOrWhiteSpace(c.Nombre))
                        .GroupBy(c => c.Nombre.Trim())
                        .Select(g => g.First())
                        .ToList();

                    foreach (var comision in comisionesUnicas)
                    {
                        ListaComisiones.Add(comision);
                    }
                }

                if (ListaComisiones.Count > 0 && ComisionSeleccionada == null)
                {
                    ComisionSeleccionada = ListaComisiones.First();
                }
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Error al cargar Comisiones]: {ex.Message}");
        }
    }

    private async Task CargarTrabajosPracticosAsync()
    {
        try
        {
            var tps = await _databaseService.GetTrabajosPracticosAsync();

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                ListaTrabajosPracticos.Clear();
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
            int? comisionId = (ComisionSeleccionada != null && ComisionSeleccionada.Id > 0) ? ComisionSeleccionada.Id : (int?)null;

            // 1. Obtenemos el reporte base según el DNI y el TP
            var resultados = await _databaseService.ObtenerReporteEntregasTpAsync(DniFiltro, tpId);

            // 2. Si hay una comisión seleccionada, filtramos los resultados cruzando los DNI de los alumnos
            if (comisionId.HasValue && comisionId.Value > 0 && ComisionSeleccionada != null)
            {
                var alumnosComision = await _databaseService.GetEstudiantesPorComisionIdAsync(comisionId.Value);

                if ((alumnosComision == null || !alumnosComision.Any()) && !string.IsNullOrWhiteSpace(ComisionSeleccionada.Nombre))
                {
                    alumnosComision = await _databaseService.GetEstudiantesPorComisionAsync(ComisionSeleccionada.Nombre);
                }

                if (alumnosComision != null && alumnosComision.Any())
                {
                    var dnisPermitidos = alumnosComision
                        .Where(a => !string.IsNullOrWhiteSpace(a.Dni))
                        .Select(a => a.Dni.Trim())
                        .ToHashSet();

                    resultados = resultados?
                        .Where(r => !string.IsNullOrEmpty(r.Dni) && dnisPermitidos.Contains(r.Dni.Trim()))
                        .ToList();
                }
                else
                {
                    resultados = new List<ReporteTpItem>();
                }
            }

            // 3. Actualizamos la interfaz gráfica en el hilo principal
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
        if (ListaComisiones.Count > 0) ComisionSeleccionada = ListaComisiones.First();
        if (ListaTrabajosPracticos.Count > 0) TpSeleccionado = ListaTrabajosPracticos.First();

        await CargarReporteAsync();
    }
}