using App_v3.Models;
using App_v3.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace App_v3.ViewModels
{
    public partial class NuevoEstudianteViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private string _nombre = string.Empty;

        [ObservableProperty]
        private string _apellido = string.Empty;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _dni = string.Empty;

        [ObservableProperty]
        private string _legajo = string.Empty;

        public NuevoEstudianteViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        [RelayCommand]
        private async Task GuardarAsync()
        {
            if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Apellido) || string.IsNullOrWhiteSpace(Dni))
            {
                if (Shell.Current is not null)
                    await Shell.Current.DisplayAlert("Atención", "Por favor completa Nombre, Apellido y DNI.", "OK");
                return;
            }

            string nombreCompleto = $"{Nombre.Trim()} {Apellido.Trim()}";

            // 1. Guardar en la tabla Persona general
            var nuevaPersona = new Persona
            {
                Nombre = Nombre.Trim(),
                Apellido = Apellido.Trim(),
                Email = Email.Trim(),
                Legajo = Legajo.Trim(),
                Rol = "Estudiante"
            };
            await _databaseService.SavePersonaAsync(nuevaPersona);

            // 2. Guardar en la tabla Estudiante para la inscripción y búsqueda por DNI
            var nuevoEstudiante = new Estudiante
            {
                NombreCompleto = nombreCompleto,
                Email = Email.Trim(),
                Dni = Dni.Trim(),
                Legajo = Legajo.Trim(),
                Activo = true
            };
            await _databaseService.SaveEstudianteAsync(nuevoEstudiante);

            // Limpiar campos
            Nombre = string.Empty;
            Apellido = string.Empty;
            Email = string.Empty;
            Dni = string.Empty;
            Legajo = string.Empty;

            if (Shell.Current is not null)
            {
                await Shell.Current.DisplayAlert("¡Guardado!", "Estudiante registrado correctamente en la base de datos.", "OK");
            }
        }

        [RelayCommand]
        private async Task VolverAlMenuAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
