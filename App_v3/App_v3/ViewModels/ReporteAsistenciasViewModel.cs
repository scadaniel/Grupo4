using System.Collections.ObjectModel;
using System.Windows.Input;
using App_v3.Models;
using App_v3.Services;

namespace App_v3.ViewModels;

public class ReporteAsistenciasViewModel : BindableObject
{
    private readonly DatabaseService _databaseService;
    private bool _isBusy;

    public ObservableCollection<Comision> Comisiones { get; set; } = new();
    public ObservableCollection<ReporteAsistenciaItem> ListaReporte { get; set; } = new();

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
                _ = CargarReporteAsync(); // Filtra automáticamente al cambiar de comisión
            }
        }
    }

    private string _dniFiltro = string.Empty;
    public string DniFiltro
    {
        get => _dniFiltro;
        set { _dniFiltro = value; OnPropertyChanged(); }
    }

    private DateTime _fechaInicio = DateTime.Today.AddMonths(-6);
    public DateTime FechaInicio
    {
        get => _fechaInicio;
        set
        {
            if (_fechaInicio != value)
            {
                _fechaInicio = value;
                OnPropertyChanged();
                _ = CargarReporteAsync(); // Filtra automáticamente al cambiar fecha inicio
            }
        }
    }

    private DateTime _fechaFin = DateTime.Today.AddDays(1);
    public DateTime FechaFin
    {
        get => _fechaFin;
        set
        {
            if (_fechaFin != value)
            {
                _fechaFin = value;
                OnPropertyChanged();
                _ = CargarReporteAsync(); // Filtra automáticamente al cambiar fecha fin
            }
        }
    }

    public ICommand BuscarCommand { get; }
    public ICommand VolverAlMenuCommand { get; }

    public ReporteAsistenciasViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;

        BuscarCommand = new Command(async () => await CargarReporteAsync());
        VolverAlMenuCommand = new Command(async () => await Shell.Current.GoToAsync(".."));

        _ = InicializarAsync();
    }

    private async Task InicializarAsync()
    {
        if (_isBusy) return;
        _isBusy = true;

        try
        {
            await CargarComisionesAsync();
            await CargarReporteAsync();
        }
        finally
        {
            _isBusy = false;
        }
    }

    private async Task CargarComisionesAsync()
    {
        try
        {
            var comisionesDb = await _databaseService.GetComisionesAsync();

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Comisiones.Clear();
                Comisiones.Add(new Comision { Id = 0, Nombre = "Todas las comisiones" });

                if (comisionesDb != null)
                {
                    foreach (var com in comisionesDb)
                    {
                        Comisiones.Add(com);
                    }
                }

                if (Comisiones.Count > 0 && ComisionSeleccionada == null)
                {
                    ComisionSeleccionada = Comisiones.First();
                }
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Error al cargar comisiones]: {ex.Message}");
        }
    }

    public async Task CargarReporteAsync()
    {
        if (_isBusy) return;
        _isBusy = true;

        try
        {
            // Extraemos el nombre de la comisión si se seleccionó una válida (Id > 0)
            string? comisionFiltro = (ComisionSeleccionada != null && ComisionSeleccionada.Id > 0) ? ComisionSeleccionada.Nombre : null;

            // Se pasa el comisionFiltro (string?) como segundo parámetro en lugar de un int?
            var resultados = await _databaseService.ObtenerReporteAsistenciasAsync(DniFiltro, comisionFiltro, FechaInicio, FechaFin);

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                ListaReporte.Clear();
                if (resultados != null)
                {
                    foreach (var item in resultados)
                    {
                        ListaReporte.Add(item);
                    }
                }
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Error al cargar reporte]: {ex.Message}");
        }
        finally
        {
            _isBusy = false;
        }
    }
}