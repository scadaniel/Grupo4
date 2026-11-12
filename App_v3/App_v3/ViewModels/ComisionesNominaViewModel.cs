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
        private bool _estaCargando = false;

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

            // ELIMINADO de aquí para evitar la doble carga inicial junto con OnAppearing():
            // _ = CargarComisionesAsync();
        }

        private async Task CargarComisionesAsync()
        {
            if (_estaCargando) return; // Bloqueo anti-ejecuciones simultáneas

            try
            {
                _estaCargando = true;
                ListaComisiones.Clear();
                var comisiones = await _databaseService.GetComisionesAsync();

                if (comisiones != null)
                {
                    // Agrupamos por nombre normalizado para asegurar elementos únicos en el Picker
                    var comisionesUnicas = comisiones
                        .Where(c => c != null && !string.IsNullOrWhiteSpace(c.Nombre))
                        .GroupBy(c => c.Nombre.Trim())
                        .Select(g => g.First())
                        .ToList();

                    foreach (var comision in comisionesUnicas)
                    {
                        ListaComisiones.Add(comision);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Error Comisiones]: {ex.Message}");
            }
            finally
            {
                _estaCargando = false;
            }
        }

        private async Task CargarAlumnosDeComisionAsync(Comision? comision)
        {
            AlumnosDeComisionSeleccionada.Clear();
            if (comision == null) return;

            try
            {
                // 1. Buscamos por el ID relacional en la tabla intermedia EstudianteComision
                var alumnosFiltrados = await _databaseService.GetEstudiantesPorComisionIdAsync(comision.Id);

                // 2. Respaldo por nombre de texto por si acaso
                if ((alumnosFiltrados == null || !alumnosFiltrados.Any()) && !string.IsNullOrWhiteSpace(comision.Nombre))
                {
                    alumnosFiltrados = await _databaseService.GetEstudiantesPorComisionAsync(comision.Nombre);
                }

                if (alumnosFiltrados != null)
                {
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

        public async Task RecargarDatosAsync()
        {
            await CargarComisionesAsync();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}