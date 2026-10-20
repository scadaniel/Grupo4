using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using App_v3.Models;
using App_v3.Services;

namespace App_v3.ViewModels;

public partial class GestionPersonasViewModel : ObservableObject
{
    private readonly IDatabaseService _db;
    private readonly IDialogService _dialogs;

    [ObservableProperty] private string nombreCompleto = string.Empty;
    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string campoSecundario = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EsModoEstudiante))]
    [NotifyPropertyChangedFor(nameof(EsModoProfesor))]
    [NotifyPropertyChangedFor(nameof(TituloPantalla))]
    [NotifyPropertyChangedFor(nameof(TituloListado))]
    private RolPersona modoActual = RolPersona.Estudiante;

    public bool EsModoEstudiante => ModoActual == RolPersona.Estudiante;
    public bool EsModoProfesor => !EsModoEstudiante;
    public string TituloPantalla => EsModoEstudiante ? "Gestión de Estudiantes" : "Gestión de Profesores";
    public string TituloListado => EsModoEstudiante ? "Estudiantes Registrados" : "Profesores Registrados";

    public ObservableCollection<Persona> ListaEstudiantes { get; } = new();
    public ObservableCollection<Persona> ListaProfesores { get; } = new();

    public GestionPersonasViewModel(IDatabaseService db, IDialogService dialogs)
    {
        _db = db;
        _dialogs = dialogs;
    }

    [RelayCommand]
    private void CambiarModo(string modo)
    {
        if (Enum.TryParse<RolPersona>(modo, out var rol))
            ModoActual = rol;
    }

    [RelayCommand]
    private async Task CargarAsync()
    {
        var personas = await _db.GetAllAsync<Persona>();

        ListaEstudiantes.Clear();
        ListaProfesores.Clear();

        foreach (var p in personas)
            (p.Rol == RolPersona.Profesor ? ListaProfesores : ListaEstudiantes).Add(p);
    }

    [RelayCommand]
    private async Task GuardarAsync()
    {
        if (string.IsNullOrWhiteSpace(NombreCompleto))
        {
            await _dialogs.AlertAsync("Atención", "Ingresa al menos un nombre.");
            return;
        }

        var partes = NombreCompleto.Trim().Split(' ', 2);

        await _db.SaveAsync(new Persona
        {
            Nombre = partes[0],
            Apellido = partes.Length > 1 ? partes[1] : string.Empty,
            Email = Email,
            Legajo = CampoSecundario,
            Rol = ModoActual
        });

        // Limpiar campos del formulario
        NombreCompleto = string.Empty;
        Email = string.Empty;
        CampoSecundario = string.Empty;

        await CargarAsync();
        await _dialogs.AlertAsync("¡Guardado!", $"Registro guardado como {ModoActual}.");
    }

    [RelayCommand]
    private async Task ResetDatabaseAsync()
    {
        await _db.ResetAsync();
        ListaEstudiantes.Clear();
        ListaProfesores.Clear();
        await _dialogs.AlertAsync("Base de Datos", "Base de datos borrada y recreada.");
    }
}