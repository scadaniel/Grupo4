using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using App_v3.Models;
using App_v3.Services;

namespace App_v3.ViewModels
{
    public class ComisionesNominaViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;

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
                    _ = CargarAlumnosDeComisionAsync(_comisionSeleccionada);
                }
            }
        }

        public ObservableCollection<Estudiante> AlumnosDeComisionSeleccionada { get; } = new();

        public ICommand VolverAlMenuCommand { get; }

        public ComisionesNominaViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            VolverAlMenuCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
            _ = CargarComisionesAsync();
        }

        private async Task CargarComisionesAsync()
        {
            try
            {
                ListaComisiones.Clear();
                var comisiones = await _databaseService.GetComisionesAsync();

                if (comisiones != null)
                {
                    foreach (var comision in comisiones)
                    {
                        ListaComisiones.Add(comision);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Error Comisiones]: {ex.Message}");
            }
        }

        private async Task CargarAlumnosDeComisionAsync(Comision? comision)
        {
            AlumnosDeComisionSeleccionada.Clear();
            if (comision == null || string.IsNullOrWhiteSpace(comision.Nombre)) return;

            try
            {
                var todosLosEstudiantes = await _databaseService.GetEstudiantesAsync();

                if (todosLosEstudiantes != null)
                {
                    string nombreLimpio = comision.Nombre.Trim();

                    var alumnosFiltrados = todosLosEstudiantes.Where(e =>
                        !string.IsNullOrWhiteSpace(e.Comision) &&
                        e.Comision.Trim().Equals(nombreLimpio, StringComparison.OrdinalIgnoreCase)
                    ).ToList();

                    foreach (var alumno in alumnosFiltrados)
                    {
                        AlumnosDeComisionSeleccionada.Add(alumno);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Error Nómina]: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}