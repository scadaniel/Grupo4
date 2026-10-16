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

        public ICommand CargarEstudiantesCommand { get; }
        public ICommand SeedEstudiantesCommand { get; }
        public ICommand ResetDatabaseCommand { get; }

        public EstudianteListViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;

            CargarEstudiantesCommand = new Command(async () => await CargarEstudiantesAsync());
            SeedEstudiantesCommand = new Command(async () => await CargarSeedDataAsync());
            ResetDatabaseCommand = new Command(async () => await BorrarBaseDeDatosAsync());
        }

        public async Task CargarEstudiantesAsync()
        {
            try
            {
                var listaBase = await _databaseService.GetEstudiantesAsync();
                Estudiantes.Clear();

                // Lista temporal para procesar y ordenar por riesgo (H8 / CA 8.1)
                var listaTemporal = new List<EstudianteItemViewModel>();

                foreach (var est in listaBase)
                {
                    // CA 6.4: Opcional, si agregaste la propiedad Activo, puedes filtrar con `if (!est.Activo) continue;`

                    // 1. Tomamos los valores reales o calculados de Inasistencias y TPs no entregados del estudiante
                    // (Si tus propiedades en la entidad Estudiante se llaman distinto, se adaptan aquí)
                    int inasistencias = est.Inasistencias;
                    int tpsNoEntregados = est.TpsNoEntregados;

                    // 2. Evaluamos el riesgo utilizando nuestro RiesgoService (H6 y H7)
                    var resultadoRiesgo = RiesgoService.CalcularRiesgo(inasistencias, tpsNoEntregados);

                    // 3. Creamos el ViewModel del ítem pasándole el riesgo calculado para la UI
                    var itemVm = new EstudianteItemViewModel(est, resultadoRiesgo.Nivel, resultadoRiesgo.ColorHex, resultadoRiesgo.Diagnostico);

                    listaTemporal.Add(itemVm);
                }

                // 4. Ordenamiento automático por nivel de riesgo: Alto (3) -> Medio (2) -> Bajo (1) (CA 8.1)
                var listaOrdenada = listaTemporal
                    .OrderByDescending(x => x.NivelRiesgoEnum)
                    .ThenBy(x => x.NombreCompleto); // Desempate alfabético por prolijidad

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
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Reset Error]: {ex.Message}");
            }
        }
    }
}