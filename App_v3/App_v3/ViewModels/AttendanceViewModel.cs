using System.Collections.ObjectModel;
using System.Windows.Input;
using App_v3.Models;
using App_v3.Services;

namespace App_v3.ViewModels
{
    public class AttendanceViewModel : BindableObject
    {
        private readonly DatabaseService _databaseService;

        private Clase? _claseSeleccionada;
        private DateTime _fechaSeleccionada = DateTime.Today; // Por defecto hoy de Windows
        private string _dniIngresado = string.Empty;

        // El Picker vuelve a alimentar objetos Clase tal como espera la vista XAML
        public ObservableCollection<Clase> ComisionesList { get; set; } = new();
        public ObservableCollection<AlumnoAsistenciaItem> AlumnosAsistencia { get; set; } = new();

        public Clase? ClaseSeleccionada
        {
            get => _claseSeleccionada;
            set
            {
                if (_claseSeleccionada != value)
                {
                    _claseSeleccionada = value;
                    OnPropertyChanged();

                    // Solo carga alumnos si realmente se seleccionó una clase válida
                    if (_claseSeleccionada != null)
                    {
                        _ = CargarAlumnosAsync();
                    }
                    else
                    {
                        AlumnosAsistencia.Clear();
                    }
                }
            }
        }

        public DateTime FechaSeleccionada
        {
            get => _fechaSeleccionada;
            set
            {
                if (_fechaSeleccionada != value)
                {
                    _fechaSeleccionada = value;
                    OnPropertyChanged();
                }
            }
        }

        public string DniIngresado
        {
            get => _dniIngresado;
            set
            {
                if (_dniIngresado != value)
                {
                    _dniIngresado = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand CargarComisionesCommand { get; }
        public ICommand GuardarAsistenciaCommand { get; }
        public ICommand InscribirAlumnoCommand { get; }

        public AttendanceViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;

            CargarComisionesCommand = new Command(async () => await CargarComisionesAsync());
            GuardarAsistenciaCommand = new Command(async () => await GuardarAsistenciaAsync());
            InscribirAlumnoCommand = new Command(async () => await InscribirAlumnoPorDniAsync());

            // Inicializamos cargando las comisiones/clases
            _ = InicializarPantallaAsync();
        }

        private async Task InicializarPantallaAsync()
        {
            await CargarComisionesAsync();
            ClaseSeleccionada = null; // Evita autoselección por defecto
            AlumnosAsistencia.Clear();
        }

        public async Task CargarComisionesAsync()
        {
            try
            {
                var listaClases = await _databaseService.GetClasesAsync();
                ComisionesList.Clear();
                if (listaClases != null)
                {
                    foreach (var clase in listaClases)
                    {
                        ComisionesList.Add(clase);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Error al cargar comisiones]: {ex.Message}");
            }
        }

        private async Task CargarAlumnosAsync()
        {
            AlumnosAsistencia.Clear();

            if (ClaseSeleccionada == null)
                return;

            try
            {
                // 1. Intentamos buscar por ID de clase en la tabla de inscripciones
                var estudiantesBD = await _databaseService.GetEstudiantesPorClaseIdAsync(ClaseSeleccionada.Id);

                // 2. Si viene vacío, respaldamos buscando por el nombre de la comisión de esta clase en los alumnos cargados
                if (estudiantesBD == null || !estudiantesBD.Any())
                {
                    if (!string.IsNullOrWhiteSpace(ClaseSeleccionada.Comision))
                    {
                        estudiantesBD = await _databaseService.GetEstudiantesPorComisionAsync(ClaseSeleccionada.Comision);
                    }
                }

                if (estudiantesBD != null)
                {
                    foreach (var est in estudiantesBD)
                    {
                        AlumnosAsistencia.Add(new AlumnoAsistenciaItem
                        {
                            EstudianteId = est.Id,
                            NombreCompleto = est.NombreCompleto,
                            Legajo = est.Legajo,
                            EstaPresente = true
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Error al cargar alumnos]: {ex.Message}");
            }
        }

        private async Task InscribirAlumnoPorDniAsync()
        {
            if (ClaseSeleccionada == null)
            {
                await Shell.Current.DisplayAlert("Atención", "Debe seleccionar una clase/comisión primero.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(DniIngresado))
            {
                await Shell.Current.DisplayAlert("Atención", "Ingrese un número de DNI válido.", "OK");
                return;
            }

            try
            {
                bool exito = await _databaseService.InscribirEstudiantePorDniAsync(DniIngresado.Trim(), ClaseSeleccionada.Id);

                if (exito)
                {
                    await Shell.Current.DisplayAlert("Éxito", "Alumno inscripto correctamente a la clase.", "OK");
                    DniIngresado = string.Empty;
                    await CargarAlumnosAsync();
                }
                else
                {
                    // Respaldo: buscamos al alumno por DNI y le asignamos la comisión por texto
                    var todosEstudiantes = await _databaseService.GetEstudiantesAsync();
                    App_v3.Models.Estudiante? estudianteEncontrado = null;

                    foreach (var e in todosEstudiantes)
                    {
                        if (e.Dni != null && e.Dni.Trim() == DniIngresado.Trim())
                        {
                            estudianteEncontrado = e;
                            break;
                        }
                    }

                    if (estudianteEncontrado != null && !string.IsNullOrWhiteSpace(ClaseSeleccionada.Comision))
                    {
                        estudianteEncontrado.Comision = ClaseSeleccionada.Comision;
                        await _databaseService.SaveEstudianteAsync(estudianteEncontrado);

                        await Shell.Current.DisplayAlert("Éxito", $"Alumno {estudianteEncontrado.NombreCompleto} asignado a la comisión correctamente.", "OK");
                        DniIngresado = string.Empty;
                        await CargarAlumnosAsync();
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Error", "No se encontró ningún estudiante activo con ese DNI.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"No se pudo realizar la inscripción: {ex.Message}", "OK");
            }
        }

        private async Task GuardarAsistenciaAsync()
        {
            if (ClaseSeleccionada == null)
            {
                await Shell.Current.DisplayAlert("Atención", "Debe seleccionar una comisión antes de guardar.", "OK");
                return;
            }

            try
            {
                var listaAsistencias = new List<Asistencia>();
                foreach (var item in AlumnosAsistencia)
                {
                    var nuevaAsistencia = new Asistencia();
                    nuevaAsistencia.ClaseId = ClaseSeleccionada.Id;
                    nuevaAsistencia.EstudianteId = item.EstudianteId;
                    nuevaAsistencia.Presente = item.EstaPresente;
                    nuevaAsistencia.Fecha = FechaSeleccionada;

                    listaAsistencias.Add(nuevaAsistencia);
                }

                await _databaseService.GuardarListaAsistenciasAsync(ClaseSeleccionada.Id, listaAsistencias);

                await Shell.Current.DisplayAlert("Éxito", $"Asistencia guardada para la fecha {FechaSeleccionada:dd/MM/yyyy}.", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"No se pudo guardar: {ex.Message}", "OK");
            }
        }
    }

    public class AlumnoAsistenciaItem : BindableObject
    {
        public int EstudianteId { get; set; }

        private string _nombreCompleto = string.Empty;
        public string NombreCompleto
        {
            get => _nombreCompleto;
            set { _nombreCompleto = value; OnPropertyChanged(); }
        }

        private string _legajo = string.Empty;
        public string Legajo
        {
            get => _legajo;
            set { _legajo = value; OnPropertyChanged(); }
        }

        private bool _estaPresente = true;
        public bool EstaPresente
        {
            get => _estaPresente;
            set { _estaPresente = value; OnPropertyChanged(); }
        }
    }
}