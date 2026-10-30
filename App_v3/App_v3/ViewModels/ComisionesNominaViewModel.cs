using App_v3.Models;
using App_v3.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace App_v3.ViewModels
{
    public partial class ComisionesNominaViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private Comision? _comisionActualSeleccionada;

        public ObservableCollection<Comision> ListaComisiones { get; set; } = new();
        public ObservableCollection<Estudiante> AlumnosDeComisionSeleccionada { get; set; } = new();

        public ComisionesNominaViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            _ = CargarComisionesAsync();
        }

        private async Task CargarComisionesAsync()
        {
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

        partial void OnComisionActualSeleccionadaChanged(Comision? value)
        {
            _ = CargarAlumnosDeComisionAsync(value);
        }

        private async Task CargarAlumnosDeComisionAsync(Comision? comision)
        {
            AlumnosDeComisionSeleccionada.Clear();
            if (comision == null) return;

            // Cambiamos comision.Id por comision.Nombre para que coincida con el método string del DatabaseService
            var alumnos = await _databaseService.GetEstudiantesPorComisionAsync(comision.Nombre);

            if (alumnos != null)
            {
                foreach (var alumno in alumnos)
                {
                    AlumnosDeComisionSeleccionada.Add(alumno);
                }
            }
        }

        [RelayCommand]
        private async Task VolverAlMenuAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}