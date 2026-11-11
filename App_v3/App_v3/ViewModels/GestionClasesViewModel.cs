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
        set
        {
            if (_nombreMateria != value)
            {
                _nombreMateria = value;
                OnPropertyChanged();
            }
        }
    }

    // Colección de comisiones para poblar el Picker
    public ObservableCollection<Comision> ListaComisiones { get; } = new();

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
            }
        }
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
        // 1. Cargar clases existentes
        var listaClases = await _databaseService.GetClasesAsync();
        Clases.Clear();
        foreach (var clase in listaClases)
        {
            Clases.Add(clase);
        }

        // 2. Cargar comisiones disponibles para el Picker
        var comisiones = await _databaseService.GetComisionesAsync();
        ListaComisiones.Clear();
        if (comisiones != null)
        {
            foreach (var com in comisiones)
            {
                ListaComisiones.Add(com);
            }
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

        if (ComisionSeleccionada == null || string.IsNullOrWhiteSpace(ComisionSeleccionada.Nombre))
        {
            if (Shell.Current != null)
                await Shell.Current.DisplayAlert("Atención", "Debe seleccionar una comisión.", "Aceptar");
            return;
        }

        var nuevaClase = new Clase
        {
            NombreMateria = NombreMateria.Trim(),
            Comision = ComisionSeleccionada.Nombre.Trim()
        };

        await _databaseService.SaveClaseYComisionNuevaAsync(nuevaClase);

        // Limpiar campos
        NombreMateria = string.Empty;
        ComisionSeleccionada = null;

        await CargarDatosAsync();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}