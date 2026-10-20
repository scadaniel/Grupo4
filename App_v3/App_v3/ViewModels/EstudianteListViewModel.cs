using System.Collections.ObjectModel;
using System.Windows.Input;
using App_v3.Models;
using App_v3.Services;

namespace App_v3.ViewModels
{
    public class EstudianteListViewModel : BindableObject
    {
        private readonly IDatabaseService _db;

        public ObservableCollection<EstudianteItemViewModel> Estudiantes { get; } = new();

        public ICommand CargarEstudiantesCommand { get; }
        public ICommand SeedEstudiantesCommand { get; }
        public ICommand ResetDatabaseCommand { get; }

        public EstudianteListViewModel(IDatabaseService db)
        {
            _db = db;

            CargarEstudiantesCommand = new Command(async () => await CargarEstudiantesAsync());
            SeedEstudiantesCommand = new Command(async () => await CargarSeedDataAsync());
            ResetDatabaseCommand = new Command(async () => await BorrarBaseDeDatosAsync());
        }

        public async Task CargarEstudiantesAsync()
        {
            try
            {
                var personas = await _db.GetAllAsync<Persona>();
                Estudiantes.Clear();

                foreach (var est in personas.Where(p => p.Rol == RolPersona.Estudiante))
                {
                    // Valores simulados deterministas según el Id, para probar los indicadores de riesgo
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
                // 0. Reiniciar la base para evitar duplicados si se presiona varias veces
                await _db.ResetAsync();

                // 1. Clases / comisiones de prueba
                var clasesEjemplo = new[]
                {
                    new Clase { NombreMateria = "Robótica y Automatización", Comision = "1K1" },
                    new Clase { NombreMateria = "Microcontroladores PIC", Comision = "2K1" },
                    new Clase { NombreMateria = "Sistemas de Control de Acceso", Comision = "3K1" }
                };

                foreach (var clase in clasesEjemplo)
                {
                    await _db.SaveAsync(clase);
                }

                // 2. Estudiantes de prueba
                var nombres = new[] { "Juan", "María", "Carlos", "Ana", "Lucas", "Sofía", "Mateo", "Lucía", "Diego", "Elena" };
                var apellidos = new[] { "Pérez", "Gómez", "Rodríguez", "López", "Martínez", "Sánchez", "Fernández", "Díaz", "Álvarez", "Torres" };

                var random = new Random(12345);

                for (int i = 1; i <= 20; i++)
                {
                    string nom = nombres[random.Next(nombres.Length)];
                    string ape = apellidos[random.Next(apellidos.Length)];

                    await _db.SaveAsync(new Persona
                    {
                        Legajo = $"LEG-{2000 + i}",
                        Nombre = nom,
                        Apellido = ape,
                        Email = $"{nom.ToLower()}.{ape.ToLower()}{i}@escuela.edu.ar",
                        Rol = RolPersona.Estudiante
                    });
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
                await _db.ResetAsync();
                Estudiantes.Clear();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Reset Error]: {ex.Message}");
            }
        }
    }
}