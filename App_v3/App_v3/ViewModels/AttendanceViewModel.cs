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
        private DateTime _fechaSeleccionada = DateTime.Today;
        private string _dniIngresado = string.Empty;

        // Propiedades para la vista previa del alumno encontrado
        private string _nombreAlumnoEncontrado = string.Empty;
        private bool _isAlumnoEncontradoVisible = false;
        private Estudiante? _estudianteParaInscribir;

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
                    if (ClaseSeleccionada != null)
                    {
                        _ = CargarAlumnosAsync();
                    }
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

        public string NombreAlumnoEncontrado
        {
            get => _nombreAlumnoEncontrado;
            set { _nombreAlumnoEncontrado = value; OnPropertyChanged(); }
        }

        public bool IsAlumnoEncontradoVisible
        {
            get => _isAlumnoEncontradoVisible;
            set { _isAlumnoEncontradoVisible = value; OnPropertyChanged(); }
        }

        public ICommand CargarComisionesCommand { get; }
        public ICommand GuardarAsistenciaCommand { get; }
        public ICommand BuscarAlumnoCommand { get; }
        public ICommand InscribirAlumnoCommand { get; }

        public AttendanceViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;

            CargarComisionesCommand = new Command(async () => await CargarComisionesAsync());
            GuardarAsistenciaCommand = new Command(async () => await GuardarAsistenciaAsync());
            BuscarAlumnoCommand = new Command(async () => await BuscarAlumnoPorDniAsync());
            InscribirAlumnoCommand = new Command(async () => await InscribirAlumnoConfirmadoAsync());

            _ = InicializarPantallaAsync();
        }

        private async Task InicializarPantallaAsync()
        {
            await CargarComisionesAsync();
            ClaseSeleccionada = null;
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
                List<Estudiante> estudiantesBD = new();

                // 1. Buscar el ID de la comisión por nombre para hacer la consulta relacional exacta
                try
                {
                    var comisiones = await _databaseService.GetComisionesAsync();
                    if (comisiones != null && !string.IsNullOrWhiteSpace(ClaseSeleccionada.Comision))
                    {
                        var comisionMatch = comisiones.FirstOrDefault(c =>
                            c.Nombre != null && c.Nombre.Trim().Equals(ClaseSeleccionada.Comision.Trim(), StringComparison.OrdinalIgnoreCase));

                        if (comisionMatch != null)
                        {
                            estudiantesBD = await _databaseService.GetEstudiantesPorComisionIdAsync(comisionMatch.Id);
                        }
                    }
                }
                catch
                {
                    // Si falla el vínculo por ID, continuamos con los respaldos por texto/clase
                }

                // 2. Respaldo por nombre de comisión directo
                if ((estudiantesBD == null || !estudiantesBD.Any()) && !string.IsNullOrWhiteSpace(ClaseSeleccionada.Comision))
                {
                    string nombreComisionLimpio = LimpiarTextoComision(ClaseSeleccionada.Comision);
                    estudiantesBD = await _databaseService.GetEstudiantesPorComisionAsync(nombreComisionLimpio);
                }

                // 3. Respaldo por ID de clase
                if (estudiantesBD == null || !estudiantesBD.Any())
                {
                    estudiantesBD = await _databaseService.GetEstudiantesPorClaseIdAsync(ClaseSeleccionada.Id);
                }

                // 4. Último respaldo: todos los estudiantes
                if (estudiantesBD == null || !estudiantesBD.Any())
                {
                    estudiantesBD = await _databaseService.GetEstudiantesAsync();
                }

                var asistenciasGuardadas = await _databaseService.GetAsistenciasPorClaseAsync(ClaseSeleccionada.Id);

                if (estudiantesBD != null)
                {
                    foreach (var est in estudiantesBD)
                    {
                        var registroPrevio = asistenciasGuardadas?
                            .FirstOrDefault(a => a.EstudianteId == est.Id && a.Fecha.Date == FechaSeleccionada.Date);

                        bool estaPresente = registroPrevio != null ? registroPrevio.Presente : true;
                        bool justificada = registroPrevio != null ? registroPrevio.Justificada : false;

                        AlumnosAsistencia.Add(new AlumnoAsistenciaItem
                        {
                            EstudianteId = est.Id,
                            NombreCompleto = est.NombreCompleto ?? "Sin Nombre",
                            Legajo = est.Legajo ?? "S/N",
                            EstaPresente = estaPresente,
                            Justificada = justificada
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Error al cargar alumnos]: {ex.Message}");
            }
        }

        private async Task BuscarAlumnoPorDniAsync()
        {
            if (string.IsNullOrWhiteSpace(DniIngresado))
            {
                await Shell.Current.DisplayAlert("Atención", "Debe cargar un número de DNI.", "OK");
                IsAlumnoEncontradoVisible = false;
                return;
            }

            try
            {
                string dniTrim = DniIngresado.Trim();
                var estudiantes = await _databaseService.GetEstudiantesAsync();
                _estudianteParaInscribir = estudiantes.FirstOrDefault(e => e.Dni != null && e.Dni.Trim() == dniTrim);

                if (_estudianteParaInscribir != null)
                {
                    NombreAlumnoEncontrado = $"Nombre: {_estudianteParaInscribir.NombreCompleto}\nLegajo: {_estudianteParaInscribir.Legajo}\nEmail: {_estudianteParaInscribir.Email ?? "No registrado"}";
                    IsAlumnoEncontradoVisible = true;
                }
                else
                {
                    _estudianteParaInscribir = null;
                    IsAlumnoEncontradoVisible = false;
                    await Shell.Current.DisplayAlert("Aviso", "El DNI ingresado no existe.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Ocurrió un error al buscar: {ex.Message}", "OK");
            }
        }

        private async Task InscribirAlumnoConfirmadoAsync()
        {
            if (ClaseSeleccionada == null)
            {
                await Shell.Current.DisplayAlert("Atención", "Debe seleccionar una clase/comisión primero.", "OK");
                return;
            }

            if (_estudianteParaInscribir == null)
            {
                await Shell.Current.DisplayAlert("Atención", "Primero debe buscar y encontrar un alumno válido.", "OK");
                return;
            }

            try
            {
                await _databaseService.InscribirEstudianteAsync(_estudianteParaInscribir.Id, ClaseSeleccionada.Id);

                if (!string.IsNullOrWhiteSpace(ClaseSeleccionada.Comision))
                {
                    string comisionLimpiada = LimpiarTextoComision(ClaseSeleccionada.Comision);
                    var comisiones = await _databaseService.GetComisionesAsync();
                    var comisionObj = comisiones.FirstOrDefault(c => c.Nombre != null && c.Nombre.Trim().Equals(comisionLimpiada, StringComparison.OrdinalIgnoreCase));
                    if (comisionObj != null)
                    {
                        await _databaseService.InscribirEstudianteEnComisionAsync(_estudianteParaInscribir.Id, comisionObj.Id);
                    }
                }

                await Shell.Current.DisplayAlert("Éxito", $"Alumno {_estudianteParaInscribir.NombreCompleto} inscripto correctamente a la clase.", "OK");

                DniIngresado = string.Empty;
                IsAlumnoEncontradoVisible = false;
                _estudianteParaInscribir = null;

                await CargarAlumnosAsync();
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
                    listaAsistencias.Add(new Asistencia
                    {
                        ClaseId = ClaseSeleccionada.Id,
                        EstudianteId = item.EstudianteId,
                        Presente = item.EstaPresente,
                        Justificada = item.Justificada,
                        Fecha = FechaSeleccionada
                    });
                }

                await _databaseService.GuardarListaAsistenciasAsync(ClaseSeleccionada.Id, listaAsistencias);

                await Shell.Current.DisplayAlert("Éxito", $"Asistencia guardada correctamente para la fecha {FechaSeleccionada:dd/MM/yyyy}.", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"No se pudo guardar: {ex.Message}", "OK");
            }
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
                {
                    limpio = limpio.Substring(inicio, fin - inicio).Trim();
                }
            }
            return limpio;
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
            set
            {
                if (_estaPresente != value)
                {
                    _estaPresente = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(EstaAusente));
                    OnPropertyChanged(nameof(EstadoTexto));
                    OnPropertyChanged(nameof(EstadoColor));
                }
            }
        }

        public bool EstaAusente
        {
            get => !_estaPresente;
            set
            {
                bool nuevoPresente = !value;
                if (_estaPresente != nuevoPresente)
                {
                    _estaPresente = nuevoPresente;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(EstaPresente));
                    OnPropertyChanged(nameof(EstadoTexto));
                    OnPropertyChanged(nameof(EstadoColor));
                }
            }
        }

        private bool _justificada = false;
        public bool Justificada
        {
            get => _justificada;
            set { _justificada = value; OnPropertyChanged(); }
        }

        public string EstadoTexto => EstaPresente ? "Presente" : "Ausente";
        public Color EstadoColor => EstaPresente ? Colors.Green : Colors.Red;
    }
}