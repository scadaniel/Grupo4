using System.Collections.ObjectModel;
using System.Windows.Input;
using App_v3.Models;
using App_v3.Services;

namespace App_v3.ViewModels;

public class GestionPersonasViewModel : BaseViewModel
{
    private readonly DatabaseService _databaseService;

    private string _nombreCompleto = string.Empty;
    public string NombreCompleto
    {
        get => _nombreCompleto;
        set => SetProperty(ref _nombreCompleto, value);
    }

    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    private string _campoSecundario = string.Empty;
    public string CampoSecundario
    {
        get => _campoSecundario;
        set => SetProperty(ref _campoSecundario, value);
    }

    private bool _esModoEstudiante = true;
    public bool EsModoEstudiante
    {
        get => _esModoEstudiante;
        set
        {
            if (SetProperty(ref _esModoEstudiante, value))
            {
                OnPropertyChanged(nameof(EsModoProfesor));
                OnPropertyChanged(nameof(TituloPantalla));
            }
        }
    }

    public bool EsModoProfesor => !EsModoEstudiante;
    public string TituloPantalla => EsModoEstudiante ? "Gestión de Estudiantes" : "Gestión de Profesores";

    public ObservableCollection<Estudiante> ListaEstudiantes { get; } = new();
    public ObservableCollection<Profesor> ListaProfesores { get; } = new();

    public ICommand CambiarModoCommand { get; }
    public ICommand GuardarCommand { get; }

    public GestionPersonasViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;

        CambiarModoCommand = new Command<string>(modo =>
        {
            EsModoEstudiante = modo == "Estudiante";
        });

        GuardarCommand = new Command(async () => await GuardarRegistroAsync());

        _ = CargarDatosAsync();
    }

    public async Task CargarDatosAsync()
    {
        var db = await _databaseService.GetConnectionAsync();

        var estudiantes = await db.Table<Estudiante>().ToListAsync();
        ListaEstudiantes.Clear();
        foreach (var e in estudiantes) ListaEstudiantes.Add(e);

        var profesores = await db.Table<Profesor>().ToListAsync();
        ListaProfesores.Clear();
        foreach (var p in profesores) ListaProfesores.Add(p);
    }

    private async Task GuardarRegistroAsync()
    {
        if (string.IsNullOrWhiteSpace(NombreCompleto) || string.IsNullOrWhiteSpace(Email))
            return;

        var db = await _databaseService.GetConnectionAsync();

        if (EsModoEstudiante)
        {
            var estudiante = new Estudiante
            {
                NombreCompleto = NombreCompleto,
                Email = Email,
                Legajo = string.IsNullOrWhiteSpace(CampoSecundario) ? "LEG-" + Random.Shared.Next(1000, 9999) : CampoSecundario
            };
            await db.InsertAsync(estudiante);
        }
        else
        {
            var profesor = new Profesor
            {
                NombreCompleto = NombreCompleto,
                Email = Email,
                Catedra = string.IsNullOrWhiteSpace(CampoSecundario) ? "General" : CampoSecundario
            };
            await db.InsertAsync(profesor);
        }

        NombreCompleto = string.Empty;
        Email = string.Empty;
        CampoSecundario = string.Empty;

        await CargarDatosAsync();
    }
}