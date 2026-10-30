using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using App_v3.Models;
using App_v3.Services;

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
            try
            {
                // Validación básica de campos obligatorios
                if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Apellido))
                {
                    if (Shell.Current is not null)
                        await Shell.Current.DisplayAlert("Atención", "El nombre y el apellido son obligatorios.", "OK");
                    return;
                }

                // Creamos la instancia del estudiante
                var nuevoEstudiante = new Estudiante
                {
                    NombreCompleto = $"{Nombre.Trim()} {Apellido.Trim()}",
                    Email = Email?.Trim(),
                    Dni = Dni?.Trim(),
                    Legajo = Legajo?.Trim()
                };

                // Guardamos en la base de datos local
                int resultado = await _databaseService.SaveEstudianteAsync(nuevoEstudiante);

                if (resultado > 0)
                {
                    // Pop-up de Éxito confirmando el impacto en SQLite
                    if (Shell.Current is not null)
                        await Shell.Current.DisplayAlert("Éxito", "El estudiante se ha registrado correctamente en la base de datos.", "OK");

                    // Regresamos a la pantalla anterior automáticamente
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    // Pop-up de Advertencia si no se afectaron filas
                    if (Shell.Current is not null)
                        await Shell.Current.DisplayAlert("Error", "No se pudo guardar el registro en la base de datos.", "OK");
                }
            }
            catch (Exception ex)
            {
                // Pop-up ante cualquier excepción inesperada
                if (Shell.Current is not null)
                    await Shell.Current.DisplayAlert("Error Crítico", $"Ocurrió un error inesperado: {ex.Message}", "OK");

                System.Diagnostics.Debug.WriteLine($"[Error al guardar estudiante]: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task VolverAlMenuAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}