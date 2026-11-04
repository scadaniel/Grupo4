using System.Windows.Input;
using App_v3.Models;
using App_v3.Services;

namespace App_v3.ViewModels;

[QueryProperty(nameof(EstudianteId), "EstudianteId")]
public class RegistrarIntervencionViewModel : BindableObject
{
    private readonly DatabaseService _databaseService;

    private int _estudianteId;
    public int EstudianteId
    {
        get => _estudianteId;
        set { _estudianteId = value; OnPropertyChanged(); }
    }

    private DateTime _fechaIntervencion = DateTime.Today;
    public DateTime FechaIntervencion
    {
        get => _fechaIntervencion;
        set { _fechaIntervencion = value; OnPropertyChanged(); }
    }

    public List<string> MediosDisponibles { get; } = new() { "Llamada", "Email", "Reunión" };

    private string _medioSeleccionado = "Llamada";
    public string MedioSeleccionado
    {
        get => _medioSeleccionado;
        set { _medioSeleccionado = value; OnPropertyChanged(); }
    }

    private string _resultadoNotas = string.Empty;
    public string ResultadoNotas
    {
        get => _resultadoNotas;
        set { _resultadoNotas = value; OnPropertyChanged(); }
    }

    public ICommand RegistrarContactoCommand { get; }

    public RegistrarIntervencionViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
        RegistrarContactoCommand = new Command(async () => await GuardarIntervencionAsync());
    }

    private async Task GuardarIntervencionAsync()
    {
        // CA 10.2: Validación de campos requeridos
        if (string.IsNullOrWhiteSpace(ResultadoNotas) || string.IsNullOrWhiteSpace(MedioSeleccionado))
        {
            await Shell.Current.DisplayAlert("Campos requeridos", "Por favor, complete el resultado/notas del contacto.", "OK");
            return;
        }

        try
        {
            var nuevaIntervencion = new IntervencionContacto
            {
                EstudianteId = EstudianteId,
                Fecha = FechaIntervencion,
                Medio = MedioSeleccionado,
                Resultado = ResultadoNotas.Trim()
            };

            await _databaseService.GuardarIntervencionAsync(nuevaIntervencion);

            await Shell.Current.DisplayAlert("Éxito", "Intervención registrada correctamente.", "OK");
            await Shell.Current.GoToAsync(".."); // Volver atrás en la navegación
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"No se pudo guardar la intervención: {ex.Message}", "OK");
        }
    }
}