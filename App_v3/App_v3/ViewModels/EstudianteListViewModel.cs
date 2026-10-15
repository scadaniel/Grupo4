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

                foreach (var est in listaBase)
                {
                    // Simulamos valores aleatorios deterministas según el Id para probar indicadores de riesgo
                    double promedioSimulado = 3.5 + (est.Id * 1.3) % 6.5;
                    int asistenciaSimulada = 50 + (est.Id * 7) % 50;

                    Estudiantes.Add(new EstudianteItemViewModel(est, promedioSimulado, asistenciaSimulada));
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
                        Email = $"{nom.ToLower()}.{ape.ToLower()}{i}@escuela.edu.ar"
                    };

                    await _databaseService.SaveEstudianteAsync(estudiante);
                }

                await CargarEstudiantesAsync();
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