using System.Collections.ObjectModel;
using System.Windows.Input;
using App_v3.Models;
using App_v3.Services;

namespace App_v3.ViewModels
{
    public class EntregaTpItemViewModel : BindableObject
    {
        public int EstudianteId { get; set; }
        public int EntregaId { get; set; }
        public string NombreEstudiante { get; set; } = string.Empty;
        public string Legajo { get; set; } = string.Empty;

        private bool _entregado = false;
        public bool Entregado
        {
            get => _entregado;
            set { _entregado = value; OnPropertyChanged(); }
        }
    }

    public class RegistroEntregasViewModel : BindableObject
    {
        private readonly DatabaseService _databaseService;

        public ObservableCollection<Clase> Clases { get; set; } = new();
        public ObservableCollection<TrabajoPractico> TrabajosPracticos { get; set; } = new();
        public ObservableCollection<EntregaTpItemViewModel> ListaAlumnos { get; set; } = new();

        private Clase? _claseSeleccionada;
        public Clase? ClaseSeleccionada
        {
            get => _claseSeleccionada;
            set
            {
                if (_claseSeleccionada != value)
                {
                    _claseSeleccionada = value;
                    OnPropertyChanged();
                    _ = AlCambiarClaseAsync();
                }
            }
        }

        private TrabajoPractico? _tpSeleccionado;
        public TrabajoPractico? TpSeleccionado
        {
            get => _tpSeleccionado;
            set
            {
                if (_tpSeleccionado != value)
                {
                    _tpSeleccionado = value;
                    OnPropertyChanged();
                    _ = CargarListaAlumnosAsync();
                }
            }
        }

        // --- Propiedades para el formulario de Creación de TP ---
        private string _nuevoTpNombre = string.Empty;
        public string NuevoTpNombre
        {
            get => _nuevoTpNombre;
            set { _nuevoTpNombre = value; OnPropertyChanged(); }
        }

        private string _nuevoTpDescripcion = string.Empty;
        public string NuevoTpDescripcion
        {
            get => _nuevoTpDescripcion;
            set { _nuevoTpDescripcion = value; OnPropertyChanged(); }
        }

        // --- Comandos ---
        public ICommand GuardarEntregasCommand { get; }
        public ICommand CrearTpCommand { get; }
        public ICommand EliminarTpCommand { get; }

        public RegistroEntregasViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;

            GuardarEntregasCommand = new Command(async () => await GuardarEntregasAsync());
            CrearTpCommand = new Command(async () => await CrearNuevoTpAsync());
            EliminarTpCommand = new Command(async () => await EliminarTpActualAsync());

            _ = CargarClasesAsync();
        }

        public async Task CargarClasesAsync()
        {
            try
            {
                var clases = await _databaseService.GetClasesAsync();
                Clases.Clear();
                foreach (var c in clases)
                {
                    Clases.Add(c);
                }

                if (Clases.Count > 0)
                {
                    ClaseSeleccionada = Clases.First();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Error cargando clases]: {ex.Message}");
            }
        }

        private async Task AlCambiarClaseAsync()
        {
            await CargarTrabajosPracticosPorComisionAsync();
            await CargarListaAlumnosAsync();
        }

        private async Task CargarTrabajosPracticosPorComisionAsync()
        {
            TrabajosPracticos.Clear();
            TpSeleccionado = null;

            if (ClaseSeleccionada == null || string.IsNullOrWhiteSpace(ClaseSeleccionada.Comision))
                return;

            try
            {
                var tps = await _databaseService.GetTrabajosPracticosPorComisionAsync(ClaseSeleccionada.Comision);
                foreach (var tp in tps)
                {
                    TrabajosPracticos.Add(tp);
                }

                if (TrabajosPracticos.Count > 0)
                {
                    TpSeleccionado = TrabajosPracticos.First();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Error cargando TPs]: {ex.Message}");
            }
        }

        public async Task CargarListaAlumnosAsync()
        {
            ListaAlumnos.Clear();

            if (ClaseSeleccionada == null)
                return;

            try
            {
                // 1. Intentamos buscar por ID de clase en inscripciones
                var estudiantes = await _databaseService.GetEstudiantesPorClaseIdAsync(ClaseSeleccionada.Id);

                // 2. Si la lista viene vacía, respaldamos buscando directamente por el texto de la comisión
                if (estudiantes == null || !estudiantes.Any())
                {
                    if (!string.IsNullOrWhiteSpace(ClaseSeleccionada.Comision))
                    {
                        estudiantes = await _databaseService.GetEstudiantesPorComisionAsync(ClaseSeleccionada.Comision);
                    }
                }

                var entregasExistentes = TpSeleccionado != null
                    ? await _databaseService.GetEntregasPorTpAsync(TpSeleccionado.Id)
                    : new List<EntregaTp>();

                foreach (var est in estudiantes)
                {
                    var entregaExistente = entregasExistentes.FirstOrDefault(e => e.EstudianteId == est.Id);

                    ListaAlumnos.Add(new EntregaTpItemViewModel
                    {
                        EstudianteId = est.Id,
                        EntregaId = entregaExistente?.Id ?? 0,
                        NombreEstudiante = est.NombreCompleto,
                        Legajo = est.Legajo,
                        Entregado = entregaExistente?.Entregado ?? false
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Error cargando alumnos]: {ex.Message}");
            }
        }

        private async Task CrearNuevoTpAsync()
        {
            if (ClaseSeleccionada == null || string.IsNullOrWhiteSpace(ClaseSeleccionada.Comision))
            {
                if (Shell.Current != null)
                    await Shell.Current.DisplayAlert("Atención", "Debe seleccionar una clase/comisión primero.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(NuevoTpNombre))
            {
                if (Shell.Current != null)
                    await Shell.Current.DisplayAlert("Atención", "Ingrese un nombre para el Trabajo Práctico.", "OK");
                return;
            }

            try
            {
                var nuevoTp = new TrabajoPractico
                {
                    Comision = ClaseSeleccionada.Comision.Trim(),
                    Nombre = NuevoTpNombre.Trim(),
                    Descripcion = NuevoTpDescripcion.Trim(),
                    FechaLimite = DateTime.Today.AddDays(14)
                };

                await _databaseService.SaveTrabajoPracticoAsync(nuevoTp);

                // Limpiar campos del formulario
                NuevoTpNombre = string.Empty;
                NuevoTpDescripcion = string.Empty;

                // Recargar lista de TPs y alumnos
                await CargarTrabajosPracticosPorComisionAsync();
                await CargarListaAlumnosAsync();

                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Éxito", $"El TP se creó correctamente para la comisión {ClaseSeleccionada.Comision}.", "OK");
                }
            }
            catch (Exception ex)
            {
                if (Shell.Current != null)
                    await Shell.Current.DisplayAlert("Error", $"No se pudo crear el TP: {ex.Message}", "OK");
            }
        }

        private async Task EliminarTpActualAsync()
        {
            if (TpSeleccionado == null)
            {
                if (Shell.Current != null)
                    await Shell.Current.DisplayAlert("Atención", "Seleccione un Trabajo Práctico para eliminar.", "OK");
                return;
            }

            bool confirmar = false;
            if (Shell.Current != null)
            {
                confirmar = await Shell.Current.DisplayAlert(
                    "Confirmar Eliminación",
                    $"¿Desea eliminar el TP '{TpSeleccionado.Nombre}' y todas sus entregas registradas?",
                    "Sí, eliminar",
                    "Cancelar");
            }

            if (!confirmar) return;

            try
            {
                await _databaseService.DeleteTrabajoPracticoAsync(TpSeleccionado);
                await AlCambiarClaseAsync();

                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Eliminado", "El Trabajo Práctico ha sido borrado con éxito.", "OK");
                }
            }
            catch (Exception ex)
            {
                if (Shell.Current != null)
                    await Shell.Current.DisplayAlert("Error", $"No se pudo eliminar el TP: {ex.Message}", "OK");
            }
        }

        public async Task GuardarEntregasAsync()
        {
            if (TpSeleccionado == null)
            {
                if (Shell.Current != null)
                    await Shell.Current.DisplayAlert("Atención", "Debe seleccionar un Trabajo Práctico.", "OK");
                return;
            }

            try
            {
                var listaAGuardar = ListaAlumnos.Select(item => new EntregaTp
                {
                    Id = item.EntregaId,
                    TrabajoPracticoId = TpSeleccionado.Id,
                    EstudianteId = item.EstudianteId,
                    Entregado = item.Entregado,
                    FechaEntrega = item.Entregado ? DateTime.Now : null
                }).ToList();

                await _databaseService.GuardarListaEntregasTpAsync(TpSeleccionado.Id, listaAGuardar);

                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Entregas Guardadas", "Se registraron las entregas del TP correctamente.", "OK");
                }
            }
            catch (Exception ex)
            {
                if (Shell.Current != null)
                    await Shell.Current.DisplayAlert("Error", $"No se pudieron guardar las entregas: {ex.Message}", "OK");
            }
        }
    }
}