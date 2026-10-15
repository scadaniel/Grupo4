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

    private string _comision = string.Empty;
    public string Comision
    {
        get => _comision;
        set
        {
            if (_comision != value)
            {
                _comision = value;
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

        CargarClasesCommand = new Command(async () => await CargarClasesAsync());
        GuardarClaseCommand = new Command(async () => await GuardarClaseAsync());

        _ = CargarClasesAsync();
    }

    private async Task CargarClasesAsync()
    {
        var lista = await _databaseService.GetClasesAsync();
        Clases.Clear();
        foreach (var clase in lista)
        {
            Clases.Add(clase);
        }
    }

    private async Task GuardarClaseAsync()
    {
        if (string.IsNullOrWhiteSpace(NombreMateria))
            return;

        var nuevaClase = new Clase
        {
            NombreMateria = NombreMateria,
            Comision = Comision
        };

        await _databaseService.SaveClaseAsync(nuevaClase);
        NombreMateria = string.Empty;
        Comision = string.Empty;
        await CargarClasesAsync();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}