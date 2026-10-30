using System.Collections.ObjectModel;
using System.Windows.Input;
using App_v3.Models;
using App_v3.Services;

namespace App_v3.ViewModels
{
    public class EstudianteListViewModel : BindableObject
    {
        private readonly DatabaseService _databaseService;

        public ObservableCollection<EstudianteItemViewModel> Estudiantes { get; set; } = new();

        // 🚀 NUEVO: Colección para llenar el Picker de comisiones en la vista
        public ObservableCollection<Comision> ListaComisiones { get; set; } = new();

        // 🚀 NUEVO: Propiedad para el estudiante seleccionado en la lista
        private EstudianteItemViewModel? _estudianteSeleccionado;
        public EstudianteItemViewModel? EstudianteSeleccionado
        {
            get => _estudianteSeleccionado;
            set
            {
                if (_estudianteSeleccionado != value)
                {
                    _estudianteSeleccionado = value;
                    OnPropertyChanged();
                }
            }
        }

        // 🚀 NUEVO: Propiedad para la comisión seleccionada en el Picker
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
                }
            }
        }

        public ICommand CargarEstudiantesCommand { get; }
        public ICommand SeedEstudiantesCommand { get; }
        public ICommand ResetDatabaseCommand { get; }

        // 🚀 NUEVO: Comando enlazado al botón "Inscribir" de tu interfaz
        public ICommand InscribirCommand { get; }

        public EstudianteListViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;

            CargarEstudiantesCommand = new Command(async () => await CargarEstudiantesAsync());
            SeedEstudiantesCommand = new Command(async () => await CargarSeedDataAsync());
            ResetDatabaseCommand = new Command(async () => await BorrarBaseDeDatosAsync());

            // Inicializamos el comando de inscripción
            InscribirCommand = new Command(async () => await InscribirEstudianteAsync());

            // Cargamos datos al iniciar
            _ = CargarDatosInicialesAsync();
        }

        private async Task CargarDatosInicialesAsync()
        {
            await CargarEstudiantesAsync();
            await CargarComisionesAsync();
        }

        public async Task CargarComisionesAsync()
        {
            try
            {
                ListaComisiones.Clear();
                var comisiones = await _databaseService.GetComisionesAsync();
                foreach (var com in comisiones)
                {
                    ListaComisiones.Add(com);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Error Comisiones]: {ex.Message}");
            }
        }

        // 🚀 NUEVO: Lógica que valida y ejecuta la inscripción evitando el error que te salía
        private async Task InscribirEstudianteAsync()
        {
            if (EstudianteSeleccionado == null || ComisionSeleccionada == null)
            {
                await Application.Current!.MainPage!.DisplayAlert("Atención", "Seleccione un estudiante y una comisión.", "OK");
                return;
            }

            try
            {
                // Dependiendo de cómo tengas nombrada la propiedad interna en EstudianteItemViewModel, 
                // probamos accediendo al modelo o actualizando su propiedad de comisión directamente:

                // Opción A: Si EstudianteItemViewModel guarda la entidad en una propiedad llamada "Estudiante" o similar,
                // o si prefieres buscar el estudiante original por ID o Legajo en la base de datos:

                // Buscamos el estudiante real en la base de datos usando el Legajo o Nombre del item seleccionado
                var listaBase = await _databaseService.GetEstudiantesAsync();
                var estudianteReal = listaBase.FirstOrDefault(e => e.NombreCompleto == EstudianteSeleccionado.NombreCompleto);
                // (O si tu EstudianteItemViewModel tiene otra propiedad de identificación, la usamos aquí)

                if (estudianteReal != null)
                {
                    estudianteReal.Comision = ComisionSeleccionada.Nombre;
                    await _databaseService.SaveEstudianteAsync(estudianteReal);

                    await Application.Current.MainPage.DisplayAlert("Éxito", $"Estudiante {estudianteReal.NombreCompleto} asignado a {ComisionSeleccionada.Nombre} correctamente.", "OK");

                    // Refrescamos la lista
                    await CargarEstudiantesAsync();
                }
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", $"No se pudo realizar la inscripción: {ex.Message}", "OK");
            }
        }
        public async Task CargarEstudiantesAsync()
        {
            try
            {
                var listaBase = await _databaseService.GetEstudiantesAsync();
                Estudiantes.Clear();

                var listaTemporal = new List<EstudianteItemViewModel>();

                foreach (var est in listaBase)
                {
                    int inasistencias = est.Inasistencias;
                    int tpsNoEntregados = est.TpsNoEntregados;

                    var resultadoRiesgo = RiesgoService.CalcularRiesgo(inasistencias, tpsNoEntregados);
                    var itemVm = new EstudianteItemViewModel(est, resultadoRiesgo.Nivel, resultadoRiesgo.ColorHex, resultadoRiesgo.Diagnostico);

                    listaTemporal.Add(itemVm);
                }

                var listaOrdenada = listaTemporal
                    .OrderByDescending(x => x.NivelRiesgoEnum)
                    .ThenBy(x => x.NombreCompleto);

                foreach (var item in listaOrdenada)
                {
                    Estudiantes.Add(item);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DB Error]: {ex.Message}");
            }
        }

        public async Task CargarSeedDataAsync()
        {
            try
            {
                await _databaseService.ResetDatabaseAsync();

                var clasesEjemplo = new[]
                {
                    new Clase { NombreMateria = "Robótica y Automatización", Comision = "1K1" },
                    new Clase { NombreMateria = "Microcontroladores PIC", Comision = "2K1" },
                    new Clase { NombreMateria = "Sistemas de Control de Acceso", Comision = "3K1" }
                };

                foreach (var clase in clasesEjemplo)
                {
                    await _databaseService.SaveClaseYComisionNuevaAsync(clase);
                }

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
                        Inasistencias = random.Next(0, 6),
                        TpsNoEntregados = random.Next(0, 4),
                        Comision = "1K1"
                    };

                    await _databaseService.SaveEstudianteAsync(estudiante);
                }

                await CargarDatosInicialesAsync();
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
                ListaComisiones.Clear();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Reset Error]: {ex.Message}");
            }
        }
    }
}