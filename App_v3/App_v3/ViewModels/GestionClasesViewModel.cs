using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using App_v3.Models;
using App_v3.Services;

namespace App_v3.ViewModels;

public class GestionClasesViewModel : INotifyPropertyChanged
{
    private readonly DatabaseService _databaseService;

    private string _nombreMateria = string.Empty;
    public string NombreMateria
    {
        get => _nombreMateria;
        set { _nombreMateria = value; OnPropertyChanged(); }
    }

    private string _comisionInput = string.Empty;
    public string ComisionInput
    {
        get => _comisionInput;
        set { _comisionInput = value; OnPropertyChanged(); }
    }

    private string _nombreProfesor = string.Empty;
    public string NombreProfesor
    {
        get => _nombreProfesor;
        set { _nombreProfesor = value; OnPropertyChanged(); }
    }

    private string _dniProfesor = string.Empty;
    public string DniProfesor
    {
        get => _dniProfesor;
        set { _dniProfesor = value; OnPropertyChanged(); }
    }

    private string _telefonoProfesor = string.Empty;
    public string TelefonoProfesor
    {
        get => _telefonoProfesor;
        set { _telefonoProfesor = value; OnPropertyChanged(); }
    }

    public ObservableCollection<Clase> Clases { get; } = new();

    public ICommand CargarClasesCommand { get; }
    public ICommand GuardarClaseCommand { get; }

    public GestionClasesViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
        CargarClasesCommand = new Command(async () => await CargarDatosAsync());
        GuardarClaseCommand = new Command(async () => await GuardarClaseAsync());

        _ = CargarDatosAsync();
    }

    private async Task CargarDatosAsync()
    {
        var listaClases = await _databaseService.GetClasesAsync();
        Clases.Clear();
        foreach (var clase in listaClases)
        {
            Clases.Add(clase);
        }
    }

    private async Task GuardarClaseAsync()
    {
        if (string.IsNullOrWhiteSpace(NombreMateria))
        {
            if (Shell.Current != null)
                await Shell.Current.DisplayAlert("Atención", "Debe ingresar el nombre de la materia.", "Aceptar");
            return;
        }

        if (string.IsNullOrWhiteSpace(ComisionInput))
        {
            if (Shell.Current != null)
                await Shell.Current.DisplayAlert("Atención", "Debe ingresar el nombre de la comisión (ej: Comisión A).", "Aceptar");
            return;
        }

        var nuevaClase = new Clase
        {
            NombreMateria = NombreMateria.Trim(),
            Comision = ComisionInput.Trim(),
            NombreProfesor = NombreProfesor.Trim(),
            DniProfesor = DniProfesor.Trim(),
            TelefonoProfesor = TelefonoProfesor.Trim()
        };

        // Guarda la clase y crea automáticamente la comisión en la tabla Comisiones si no existe
        await _databaseService.SaveClaseYComisionNuevaAsync(nuevaClase);

        // Limpiar formulario
        NombreMateria = string.Empty;
        ComisionInput = string.Empty;
        NombreProfesor = string.Empty;
        DniProfesor = string.Empty;
        TelefonoProfesor = string.Empty;

        await CargarDatosAsync();

        if (Shell.Current != null)
            await Shell.Current.DisplayAlert("Éxito", "Clase y comisión guardadas correctamente.", "Aceptar");
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}