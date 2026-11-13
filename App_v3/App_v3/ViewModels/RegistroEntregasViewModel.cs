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

        private bool _entregado;
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

        private string LimpiarTextoComision(string textoComision)
        {
            if (string.IsNullOrWhiteSpace(textoComision))
                return string.Empty;

            string limpio = textoComision.Trim();
            if (limpio.Contains("[") && limpio.Contains("]"))
            {
                int inicio = limpio.IndexOf('[') + 1;
                int fin = limpio.IndexOf(']');
                if (fin > inicio)
                    limpio = limpio.Substring(inicio, fin - inicio).Trim();
            }
            return limpio;
        }

        public async Task CargarClasesAsync()
        {
            try
            {
                var clases = await _databaseService.GetClasesAsync();
                Clases.Clear();
                foreach (var c in clases)
                    Clases.Add(c);

                if (Clases.Count > 0 && ClaseSeleccionada == null)
                    ClaseSeleccionada = Clases.First();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Error cargando clases TP]: {ex.Message}");
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
                string comisionLimpia = LimpiarTextoComision(ClaseSeleccionada.Comision);
                var tps = await _databaseService.GetTrabajosPracticosPorComisionAsync(comisionLimpia);

                // Respaldo por si el texto original no coincide tras limpiar
                if (tps == null || !tps.Any())
                    tps = await _databaseService.GetTrabajosPracticosPorComisionAsync(ClaseSeleccionada.Comision);

                if (tps != null)
                {
                    foreach (var tp in tps)
                        TrabajosPracticos.Add(tp);
                }

                if (TrabajosPracticos.Count > 0)
                    TpSeleccionado = TrabajosPracticos.First();
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
                // LLAMADA 100% EXCLUSIVA: No toca asistencias ni altera otras tablas
                List<Estudiante> estudiantesBD = await _databaseService.GetEstudiantesParaTpExclusivoAsync(
                    ClaseSeleccionada.Id,
                    ClaseSeleccionada.Comision ?? string.Empty
                );

                // Obtener las entregas existentes para este TP específico
                var entregasExistentes = TpSeleccionado != null
                    ? await _databaseService.GetEntregasPorTpAsync(TpSeleccionado.Id)
                    : new List<EntregaTp>();

                if (estudiantesBD != null)
                {
                    foreach (var est in estudiantesBD)
                    {
                        var entregaExistente = entregasExistentes
                            .FirstOrDefault(e => e.EstudianteId == est.Id);

                        ListaAlumnos.Add(new EntregaTpItemViewModel
                        {
                            EstudianteId = est.Id,
                            EntregaId = entregaExistente?.Id ?? 0,
                            NombreEstudiante = est.NombreCompleto ?? "Sin Nombre",
                            Legajo = est.Legajo ?? "S/N",
                            Entregado = entregaExistente?.Entregado ?? false
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Error exclusivo cargando alumnos TP]: {ex.Message}");
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
                string comisionLimpia = LimpiarTextoComision(ClaseSeleccionada.Comision);

                var nuevoTp = new TrabajoPractico
                {
                    Comision = string.IsNullOrWhiteSpace(comisionLimpia)
                        ? ClaseSeleccionada.Comision.Trim()
                        : comisionLimpia,
                    Nombre = NuevoTpNombre.Trim(),
                    Descripcion = NuevoTpDescripcion?.Trim() ?? string.Empty,
                    FechaLimite = DateTime.Today.AddDays(14)
                };

                await _databaseService.SaveTrabajoPracticoAsync(nuevoTp);

                NuevoTpNombre = string.Empty;
                NuevoTpDescripcion = string.Empty;

                await CargarTrabajosPracticosPorComisionAsync();
                await CargarListaAlumnosAsync();

                // Actualizar contadores de pendientes de los alumnos de la comisión
                foreach (var item in ListaAlumnos)
                    await _databaseService.RecalcularTpsPendientesEstudianteRobustoAsync(item.EstudianteId);

                if (Shell.Current != null)
                    await Shell.Current.DisplayAlert("Éxito",
                        $"El TP se creó correctamente para la comisión {nuevoTp.Comision}.", "OK");
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
                // Guardamos ids de alumnos para recalcular después
                var idsAlumnos = ListaAlumnos.Select(a => a.EstudianteId).ToList();

                await _databaseService.DeleteTrabajoPracticoAsync(TpSeleccionado);
                await AlCambiarClaseAsync();

                foreach (var id in idsAlumnos)
                    await _databaseService.RecalcularTpsPendientesEstudianteRobustoAsync(id);

                if (Shell.Current != null)
                    await Shell.Current.DisplayAlert("Eliminado", "El Trabajo Práctico ha sido borrado con éxito.", "OK");
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

            if (!ListaAlumnos.Any())
            {
                if (Shell.Current != null)
                    await Shell.Current.DisplayAlert("Atención", "No hay alumnos en la lista para guardar.", "OK");
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

                // Método NUEVO: no toca GuardarListaEntregasTpAsync ni Asistencias
                await _databaseService.GuardarEntregasTrabajosPracticosAsync(TpSeleccionado.Id, listaAGuardar);

                // Refrescar lista (para actualizar EntregaId de los recién insertados)
                await CargarListaAlumnosAsync();

                if (Shell.Current != null)
                    await Shell.Current.DisplayAlert("Entregas Guardadas",
                        "Se registraron las entregas del TP y se actualizó el contador de pendientes.", "OK");
            }
            catch (Exception ex)
            {
                if (Shell.Current != null)
                    await Shell.Current.DisplayAlert("Error", $"No se pudieron guardar las entregas: {ex.Message}", "OK");
            }
        }
    }
}