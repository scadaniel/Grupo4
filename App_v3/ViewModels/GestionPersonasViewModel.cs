using App_v3.Models;
using App_v3.Services;

using System.Collections.ObjectModel;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace App_v3.ViewModels;

public class GestionPersonasViewModel : INotifyPropertyChanged
{
    private readonly DatabaseService _databaseService;

    private string _nombreCompleto = string.Empty;
    public string NombreCompleto
    {
        get => _nombreCompleto;
        set { _nombreCompleto = value; OnPropertyChanged(); }
    }

    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set { _email = value; OnPropertyChanged(); }
    }

    private string _campoSecundario = string.Empty;
    public string CampoSecundario
    {
        get => _campoSecundario;
        set { _campoSecundario = value; OnPropertyChanged(); }
    }

    private string _modoActual = "Estudiante";

    private bool _esModoEstudiante = true;
    public bool EsModoEstudiante
    {
        get => _esModoEstudiante;
        set { _esModoEstudiante = value; OnPropertyChanged(); }
    }

    private bool _esModoProfesor = false;
    public bool EsModoProfesor
    {
        get => _esModoProfesor;
        set { _esModoProfesor = value; OnPropertyChanged(); }
    }

    private string _tituloPantalla = "Gestión de Estudiantes";
    public string TituloPantalla
    {
        get => _tituloPantalla;
        set { _tituloPantalla = value; OnPropertyChanged(); }
    }

    private string _tituloListado = "Estudiantes Registrados";
    public string TituloListado
    {
        get => _tituloListado;
        set { _tituloListado = value; OnPropertyChanged(); }
    }

    public ObservableCollection<Persona> ListaEstudiantes { get; set; } = new();
    public ObservableCollection<Persona> ListaProfesores { get; set; } = new();

    public ICommand CambiarModoCommand { get; }
    public ICommand GuardarCommand { get; }
    public ICommand ResetDatabaseCommand { get; }

    public GestionPersonasViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;

        CambiarModoCommand = new Command<string>(CambiarModo);
        GuardarCommand = new Command(async () => await GuardarRegistro());
        ResetDatabaseCommand = new Command(async () => await ResetearBD());

        _ = CargarDatos();
    }

    private void CambiarModo(string modo)
    {
        if (string.IsNullOrEmpty(modo)) return;

        _modoActual = modo;
        if (modo == "Estudiante")
        {
            EsModoEstudiante = true;
            EsModoProfesor = false;
            TituloPantalla = "Gestión de Estudiantes";
            TituloListado = "Estudiantes Registrados";
        }
        else
        {
            EsModoEstudiante = false;
            EsModoProfesor = true;
            TituloPantalla = "Gestión de Profesores";
            TituloListado = "Profesores Registrados";
        }
    }

    private async Task CargarDatos()
    {
        var personas = await _databaseService.GetPersonasAsync();

        ListaEstudiantes.Clear();
        ListaProfesores.Clear();

        foreach (var p in personas)
        {
            if (p.Rol == "Profesor")
            {
                ListaProfesores.Add(p);
            }
            else
            {
                ListaEstudiantes.Add(p);
            }
        }
    }

    private async Task GuardarRegistro()
    {
        if (string.IsNullOrWhiteSpace(NombreCompleto))
        {
            if (Shell.Current is not null)
                await Shell.Current.DisplayAlert("Atención", "Ingresa al menos un nombre.", "OK");
            return;
        }

        var partes = NombreCompleto.Trim().Split(' ', 2);
        string nombre = partes[0];
        string apellido = partes.Length > 1 ? partes[1] : string.Empty;

        var nuevaPersona = new Persona
        {
            Nombre = nombre,
            Apellido = apellido,
            Email = Email,
            Legajo = CampoSecundario,
            Rol = _modoActual
        };

        await _databaseService.SavePersonaAsync(nuevaPersona);

        NombreCompleto = string.Empty;
        Email = string.Empty;
        CampoSecundario = string.Empty;

        await CargarDatos();

        if (Shell.Current is not null)
        {
            await Shell.Current.DisplayAlert("¡Guardado!", $"Registro guardado como {_modoActual} en SQLite.", "OK");
        }
    }

    private async Task ResetearBD()
    {
        await _databaseService.ResetDatabaseAsync();
        ListaEstudiantes.Clear();
        ListaProfesores.Clear();

        if (Shell.Current is not null)
        {
            await Shell.Current.DisplayAlert("Base de Datos", "Base de datos borrada y recreada.", "OK");
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}