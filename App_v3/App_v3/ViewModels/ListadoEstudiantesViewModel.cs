using App_v3.Models;
using App_v3.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace App_v3.ViewModels
{
    public partial class ListadoEstudiantesViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;
        private List<Estudiante> _todosLosEstudiantes = new();

        [ObservableProperty]
        private string _filtroApellido = string.Empty;

        [ObservableProperty]
        private string _filtroDni = string.Empty;

        [ObservableProperty]
        private Comision? _comisionSeleccionada;

        public ObservableCollection<Estudiante> ListaEstudiantesFiltrada { get; set; } = new();
        public ObservableCollection<Comision> ListaComisiones { get; set; } = new();

        public ListadoEstudiantesViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            _ = CargarDatosAsync();
        }

        private async Task CargarDatosAsync()
        {
            var estudiantes = await _databaseService.GetEstudiantesAsync();
            _todosLosEstudiantes = estudiantes ?? new List<Estudiante>();
            AplicarFiltros();

            var comisiones = await _databaseService.GetComisionesAsync();
            ListaComisiones.Clear();
            if (comisiones != null)
            {
                foreach (var c in comisiones)
                {
                    ListaComisiones.Add(c);
                }
            }
        }

        partial void OnFiltroApellidoChanged(string value) => AplicarFiltros();
        partial void OnFiltroDniChanged(string value) => AplicarFiltros();

        private void AplicarFiltros()
        {
            var query = _todosLosEstudiantes.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(FiltroApellido))
            {
                query = query.Where(e => e.NombreCompleto != null &&
                                         e.NombreCompleto.Contains(FiltroApellido, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(FiltroDni))
            {
                query = query.Where(e => e.Dni != null && e.Dni.Contains(FiltroDni));
            }

            ListaEstudiantesFiltrada.Clear();
            foreach (var est in query)
            {
                ListaEstudiantesFiltrada.Add(est);
            }
        }

        [RelayCommand]
        private async Task InscribirComisionAsync(Estudiante estudiante)
        {
            if (estudiante == null) return;

            if (ComisionSeleccionada == null)
            {
                if (Shell.Current is not null)
                {
                    await Shell.Current.DisplayAlert("Error", "Debe seleccionar una comisión antes de inscribir al estudiante.", "Aceptar");
                }
                return;
            }

            try
            {
                // Guardamos formalmente la relación en la tabla intermedia de SQLite
                await _databaseService.InscribirEstudianteEnComisionAsync(estudiante.Id, ComisionSeleccionada.Id);

                if (Shell.Current is not null)
                    await Shell.Current.DisplayAlert("Éxito", $"¡Se inscribió correctamente a {estudiante.NombreCompleto} en la comisión {ComisionSeleccionada.Nombre}!", "Aceptar");
            }
            catch (Exception ex)
            {
                if (Shell.Current is not null)
                    await Shell.Current.DisplayAlert("Error", $"No se pudo completar la inscripción: {ex.Message}", "Aceptar");
            }
        }

        [RelayCommand]
        private async Task VolverAlMenuAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}