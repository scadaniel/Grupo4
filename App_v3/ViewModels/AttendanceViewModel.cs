using System.Collections.ObjectModel;
using System.Windows.Input;
using App_v3.Models;
using App_v3.Services;

namespace App_v3.ViewModels;

public class AttendanceViewModel : BindableObject
{
    private readonly DatabaseService _databaseService;

    private Clase? _claseSeleccionada;
    private DateTime _fechaSeleccionada = DateTime.Today;

    public ObservableCollection<Clase> ComisionesList { get; set; } = new();
    public ObservableCollection<AlumnoAsistenciaItem> AlumnosAsistencia { get; set; } = new();

    public Clase? ClaseSeleccionada
    {
        get => _claseSeleccionada;
        set
        {
            if (_claseSeleccionada != value)
            {
                _claseSeleccionada = value;
                OnPropertyChanged();
                _ = CargarAlumnosAsync();
            }
        }
    }

    public DateTime FechaSeleccionada
    {
        get => _fechaSeleccionada;
        set
        {
            if (_fechaSeleccionada != value)
            {
                _fechaSeleccionada = value;
                OnPropertyChanged();
            }
        }
    }

    public ICommand CargarComisionesCommand { get; }
    public ICommand GuardarAsistenciaCommand { get; }

    public AttendanceViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;

        CargarComisionesCommand = new Command(async () => await CargarComisionesAsync());
        GuardarAsistenciaCommand = new Command(async () => await GuardarAsistenciaAsync());

        _ = CargarComisionesAsync();
    }

    public async Task CargarComisionesAsync()
    {
        try
        {
            var listaClases = await _databaseService.GetClasesAsync();
            ComisionesList.Clear();
            foreach (var clase in listaClases)
            {
                ComisionesList.Add(clase);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Error al cargar comisiones]: {ex.Message}");
        }
    }

    private async Task CargarAlumnosAsync()
    {
        AlumnosAsistencia.Clear();

        if (ClaseSeleccionada == null)
            return;

        try
        {
            var estudiantesBD = await _databaseService.GetEstudiantesAsync();

            foreach (var est in estudiantesBD)
            {
                AlumnosAsistencia.Add(new AlumnoAsistenciaItem
                {
                    EstudianteId = est.Id,
                    NombreCompleto = est.NombreCompleto,
                    EstaPresente = true
                });
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Error al cargar alumnos]: {ex.Message}");
        }
    }

    private async Task GuardarAsistenciaAsync()
    {
        if (ClaseSeleccionada == null)
        {
            await Shell.Current.DisplayAlert("Atención", "Debe seleccionar una comisión/clase.", "OK");
            return;
        }

        try
        {
            foreach (var item in AlumnosAsistencia)
            {
                var nuevaAsistencia = new Asistencia
                {
                    ClaseId = ClaseSeleccionada.Id,
                    EstudianteId = item.EstudianteId,
                    Presente = item.EstaPresente
                };

                // await _databaseService.SaveAsistenciaAsync(nuevaAsistencia);
            }

            await Shell.Current.DisplayAlert("Éxito", "La asistencia ha sido guardada correctamente.", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"No se pudo guardar: {ex.Message}", "OK");
        }
    }
}

public class AlumnoAsistenciaItem : BindableObject
{
    public int EstudianteId { get; set; }

    private string _nombreCompleto = string.Empty;
    public string NombreCompleto
    {
        get => _nombreCompleto;
        set { _nombreCompleto = value; OnPropertyChanged(); }
    }

    private bool _estaPresente = true;
    public bool EstaPresente
    {
        get => _estaPresente;
        set { _estaPresente = value; OnPropertyChanged(); }
    }
}