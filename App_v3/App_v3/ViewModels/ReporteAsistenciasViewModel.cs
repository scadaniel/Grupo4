using System.Collections.ObjectModel;
using System.Windows.Input;
using App_v3.Models;
using App_v3.Services;
using App_v3.Views;

namespace App_v3.ViewModels;

public class ReporteAsistenciasViewModel : BindableObject
{
    private readonly DatabaseService _databaseService;

    public ObservableCollection<Clase> Clases { get; set; } = new();
    public ObservableCollection<ReporteAsistenciaItem> ListaReporte { get; set; } = new();

    private Clase? _claseSeleccionada;
    public Clase? ClaseSeleccionada
    {
        get => _claseSeleccionada;
        set { _claseSeleccionada = value; OnPropertyChanged(); }
    }

    private string _dniFiltro = string.Empty;
    public string DniFiltro
    {
        get => _dniFiltro;
        set { _dniFiltro = value; OnPropertyChanged(); }
    }

    private DateTime _fechaInicio = DateTime.Today.AddMonths(-2);
    public DateTime FechaInicio
    {
        get => _fechaInicio;
        set { _fechaInicio = value; OnPropertyChanged(); }
    }

    private DateTime _fechaFin = DateTime.Today;
    public DateTime FechaFin
    {
        get => _fechaFin;
        set { _fechaFin = value; OnPropertyChanged(); }
    }

    public ICommand BuscarCommand { get; }
    public ICommand LimpiarFiltrosCommand { get; }
    public ICommand VolverAlMenuCommand { get; }

    public ReporteAsistenciasViewModel(DatabaseService databaseService)
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
        await CargarClasesAsync();
        await CargarReporteAsync();
    }

    private async Task CargarClasesAsync()
    {
        var clases = await _databaseService.GetClasesAsync();
        Clases.Clear();

        // Opción comodín para ver todas las clases
        Clases.Add(new Clase { Id = 0, NombreMateria = "Todas las clases / comisiones", Comision = "General" });

        foreach (var c in clases)
        {
            Clases.Add(c);
        }
        ClaseSeleccionada = Clases.First();
    }

    public async Task CargarReporteAsync()
    {
        int? claseId = (ClaseSeleccionada != null && ClaseSeleccionada.Id > 0) ? ClaseSeleccionada.Id : (int?)null;

        var resultados = await _databaseService.ObtenerReporteAsistenciasAsync(DniFiltro, claseId, FechaInicio, FechaFin);

        ListaReporte.Clear();
        foreach (var item in resultados)
        {
            ListaReporte.Add(item);
        }
    }

    private async Task LimpiarFiltrosAsync()
    {
        DniFiltro = string.Empty;
        if (Clases.Count > 0) ClaseSeleccionada = Clases.First();
        FechaInicio = DateTime.Today.AddMonths(-2);
        FechaFin = DateTime.Today;
        await CargarReporteAsync();
    }
}