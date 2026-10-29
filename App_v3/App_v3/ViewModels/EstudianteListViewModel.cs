using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using App_v3.Core;
using App_v3.Models;
using App_v3.Services;

namespace App_v3.ViewModels
{
    // Panel de seguimiento. Historias H8 (ordenado por riesgo), H10 (registrar contacto)
    // y H11 (mostrar a quién ya se contactó y filtrar a los que no).
    public class EstudianteListViewModel : BindableObject
    {
        private readonly DatabaseService _databaseService;
        private readonly SeguimientoContactos _seguimiento;

        private EstudianteItemViewModel? _estudianteEnContacto;
        private bool _soloNoContactados;
        private bool _formularioVisible;
        private string _tituloFormulario = string.Empty;
        private DateTime _fechaContacto = DateTime.Today;
        private string _medioSeleccionado = "Llamada";
        private string? _resultadoSeleccionado;
        private string _nota = string.Empty;
        private string _mensajeError = string.Empty;
        private string _emailContacto = string.Empty;
        private string _telefonoContacto = string.Empty;

        public ObservableCollection<EstudianteItemViewModel> Estudiantes { get; set; } = new();

        public ICommand CargarEstudiantesCommand { get; }
        public ICommand SeedEstudiantesCommand { get; }
        public ICommand ResetDatabaseCommand { get; }
        public ICommand AlternarFiltroCommand { get; }
        public ICommand RegistrarContactoCommand { get; }
        public ICommand CancelarContactoCommand { get; }
        public ICommand EnviarEmailCommand { get; }
        public ICommand LlamarCommand { get; }

        // Opciones del formulario. Si el equipo quiere otros resultados, se cambian acá.
        public List<string> Medios { get; } = new() { "Llamada", "Email" };

        public List<string> Resultados { get; } = new()
        {
            "Respondió",
            "No respondió",
            "Se acordó un compromiso",
            "Derivado a tutoría"
        };

        public bool SoloNoContactados
        {
            get => _soloNoContactados;
            private set
            {
                Asignar(ref _soloNoContactados, value);
                OnPropertyChanged(nameof(TextoFiltro));
            }
        }

        public string TextoFiltro => SoloNoContactados ? "Ver todos" : "No contactados esta semana";

        public bool FormularioVisible
        {
            get => _formularioVisible;
            private set => Asignar(ref _formularioVisible, value);
        }

        public string TituloFormulario
        {
            get => _tituloFormulario;
            private set => Asignar(ref _tituloFormulario, value);
        }

        public DateTime FechaContacto
        {
            get => _fechaContacto;
            set => Asignar(ref _fechaContacto, value);
        }

        public string MedioSeleccionado
        {
            get => _medioSeleccionado;
            set => Asignar(ref _medioSeleccionado, value);
        }

        public string? ResultadoSeleccionado
        {
            get => _resultadoSeleccionado;
            set => Asignar(ref _resultadoSeleccionado, value);
        }

        public string Nota
        {
            get => _nota;
            set => Asignar(ref _nota, value);
        }

        public string MensajeError
        {
            get => _mensajeError;
            private set => Asignar(ref _mensajeError, value);
        }

        // Datos para saber cómo ubicar al estudiante elegido.
        public bool TieneEmail => !string.IsNullOrWhiteSpace(_emailContacto);
        public bool TieneTelefono => !string.IsNullOrWhiteSpace(_telefonoContacto);

        public string TextoEmailContacto =>
            TieneEmail ? $"Email: {_emailContacto}" : "Sin email cargado";

        public string TextoTelefonoContacto =>
            TieneTelefono ? $"Teléfono: {_telefonoContacto}" : "Sin teléfono cargado";

        public EstudianteListViewModel(DatabaseService databaseService, SeguimientoContactos seguimiento)
        {
            _databaseService = databaseService;
            _seguimiento = seguimiento;

            CargarEstudiantesCommand = new Command(async () => await CargarEstudiantesAsync());
            SeedEstudiantesCommand = new Command(async () => await CargarSeedDataAsync());
            ResetDatabaseCommand = new Command(async () => await BorrarBaseDeDatosAsync());
            AlternarFiltroCommand = new Command(async () => await AlternarFiltroAsync());
            RegistrarContactoCommand = new Command(async () => await RegistrarContactoAsync());
            CancelarContactoCommand = new Command(CerrarFormulario);
            EnviarEmailCommand = new Command(async () => await EnviarEmailAsync());
            LlamarCommand = new Command(async () => await LlamarAsync());
        }

        public async Task CargarEstudiantesAsync()
        {
            try
            {
                var estudiantes = await _databaseService.GetEstudiantesAsync();
                var ultimosContactos = await _seguimiento.UltimosContactosAsync();

                var filas = new List<EstudianteItemViewModel>();

                // Solo estudiantes activos: los dados de baja no entran al panel (criterio 6.4).
                foreach (var estudiante in estudiantes.Where(e => e.Activo))
                {
                    var riesgo = RiesgoService.CalcularRiesgo(estudiante.Inasistencias, estudiante.TpsNoEntregados);
                    DateTime? ultimoContacto = ultimosContactos.TryGetValue(estudiante.Id, out var fecha)
                        ? fecha
                        : (DateTime?)null;

                    filas.Add(new EstudianteItemViewModel(
                        estudiante, riesgo.Nivel, riesgo.ColorHex, riesgo.Diagnostico,
                        ultimoContacto, AbrirFormulario));
                }

                if (SoloNoContactados)
                {
                    var sinContacto = await _seguimiento.NoContactadosEstaSemanaAsync(
                        filas.Select(f => (f.Estudiante.Id, f.NivelRiesgoEnum)));
                    filas = filas.Where(f => sinContacto.Contains(f.Estudiante.Id)).ToList();
                }

                // De mayor a menor riesgo, y por nombre cuando hay empate (criterio 8.1).
                Estudiantes.Clear();
                foreach (var fila in filas.OrderByDescending(f => f.NivelRiesgoEnum).ThenBy(f => f.NombreCompleto))
                {
                    Estudiantes.Add(fila);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DB Error]: {ex.Message}");
            }
        }

        private async Task AlternarFiltroAsync()
        {
            SoloNoContactados = !SoloNoContactados;
            await CargarEstudiantesAsync();
        }

        private void AbrirFormulario(EstudianteItemViewModel estudiante)
        {
            _estudianteEnContacto = estudiante;
            TituloFormulario = $"Contacto con {estudiante.NombreCompleto}";
            FechaContacto = DateTime.Today;
            MedioSeleccionado = "Llamada";
            ResultadoSeleccionado = null;
            Nota = string.Empty;
            MensajeError = string.Empty;

            _emailContacto = estudiante.Email;
            _telefonoContacto = estudiante.Estudiante.Telefono;
            OnPropertyChanged(nameof(TieneEmail));
            OnPropertyChanged(nameof(TieneTelefono));
            OnPropertyChanged(nameof(TextoEmailContacto));
            OnPropertyChanged(nameof(TextoTelefonoContacto));

            FormularioVisible = true;
        }

        private void CerrarFormulario()
        {
            _estudianteEnContacto = null;
            FormularioVisible = false;
            MensajeError = string.Empty;
        }

        // Criterios 10.1 y 10.2: guarda el contacto o avisa qué campos faltan.
        private async Task RegistrarContactoAsync()
        {
            if (_estudianteEnContacto is null)
                return;

            var nuevo = new NuevoContacto(
                _estudianteEnContacto.Estudiante.Id,
                FechaContacto,
                Enum.Parse<MedioContacto>(MedioSeleccionado),
                ResultadoSeleccionado,
                Nota);

            try
            {
                await _seguimiento.RegistrarAsync(nuevo);
            }
            catch (ContactoInvalidoException ex)
            {
                MensajeError = "Faltan completar: " + string.Join(", ", ex.CamposFaltantes) + ".";
                return;
            }

            CerrarFormulario();
            await CargarEstudiantesAsync();
        }

        // Abre el programa de correo con el email del estudiante y deja elegido el medio "Email".
        // Después de enviarlo, el responsable completa el resultado y registra el contacto.
        private async Task EnviarEmailAsync()
        {
            if (!TieneEmail)
                return;

            MedioSeleccionado = "Email";
            var asunto = Uri.EscapeDataString("Seguimiento de la cursada");
            await AbrirEnlaceAsync(
                $"mailto:{_emailContacto}?subject={asunto}",
                "No se pudo abrir el programa de correo.");
        }

        // Intenta iniciar la llamada. En una PC sin aplicación de llamadas avisa y deja el número a la vista.
        private async Task LlamarAsync()
        {
            if (!TieneTelefono)
                return;

            MedioSeleccionado = "Llamada";
            var numero = new string(_telefonoContacto.Where(c => char.IsDigit(c) || c == '+').ToArray());
            await AbrirEnlaceAsync(
                $"tel:{numero}",
                $"Esta computadora no puede iniciar llamadas. Llamá al {_telefonoContacto} desde otro teléfono.");
        }

        private async Task AbrirEnlaceAsync(string direccion, string mensajeSiFalla)
        {
            MensajeError = string.Empty;

            try
            {
                var seAbrio = await Launcher.Default.OpenAsync(new Uri(direccion));
                if (!seAbrio)
                    MensajeError = mensajeSiFalla;
            }
            catch (Exception)
            {
                MensajeError = mensajeSiFalla;
            }
        }

        public async Task CargarSeedDataAsync()
        {
            try
            {
                // 0. Reiniciar la base de datos para evitar duplicados si se presiona varias veces
                await _databaseService.ResetDatabaseAsync();

                // 1. CREAR CLASES / COMISIONES DE PRUEBA
                var clasesEjemplo = new[]
                {
                    new Clase { NombreMateria = "Robótica y Automatización", Comision = "1K1" },
                    new Clase { NombreMateria = "Microcontroladores PIC", Comision = "2K1" },
                    new Clase { NombreMateria = "Sistemas de Control de Acceso", Comision = "3K1" }
                };

                foreach (var clase in clasesEjemplo)
                {
                    await _databaseService.SaveClaseAsync(clase);
                }

                // 2. CREAR ESTUDIANTES DE PRUEBA (con datos variados de faltas y TPs para probar el ordenamiento)
                var nombres = new[] { "Juan", "María", "Carlos", "Ana", "Lucas", "Sofía", "Mateo", "Lucía", "Diego", "Elena" };
                var apellidos = new[] { "Pérez", "Gómez", "Rodríguez", "López", "Martínez", "Sánchez", "Fernández", "Díaz", "Álvarez", "Torres" };

                var random = new Random(12345);

                for (int i = 1; i <= 20; i++)
                {
                    string nom = nombres[random.Next(nombres.Length)];
                    string ape = apellidos[random.Next(apellidos.Length)];

                    var estudiante = new Estudiante
                    {
                        Legajo = $"LEG-{2000 + i}",
                        NombreCompleto = $"{nom} {ape}",
                        Email = $"{nom.ToLower()}.{ape.ToLower()}{i}@escuela.edu.ar",
                        Telefono = $"11 5555 {1000 + i}",
                        // Simulamos faltas y TPs para probar el motor de riesgo inmediatamente al hacer Seed
                        Inasistencias = random.Next(0, 6),
                        TpsNoEntregados = random.Next(0, 4)
                    };

                    await _databaseService.SaveEstudianteAsync(estudiante);
                }

                // 3. Recargar la lista en pantalla
                await CargarEstudiantesAsync();

                System.Diagnostics.Debug.WriteLine("[Seed Exitoso]: Clases y Estudiantes cargados correctamente.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Seed Error]: {ex.Message}");
            }
        }

        public async Task BorrarBaseDeDatosAsync()
        {
            try
            {
                await _databaseService.ResetDatabaseAsync();
                Estudiantes.Clear();
                CerrarFormulario();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Reset Error]: {ex.Message}");
            }
        }

        // Cambia el valor de un campo y avisa a la pantalla solo si realmente cambió.
        private void Asignar<T>(ref T campo, T valor, [CallerMemberName] string? nombre = null)
        {
            if (EqualityComparer<T>.Default.Equals(campo, valor))
                return;

            campo = valor;
            OnPropertyChanged(nombre);
        }
    }
}
