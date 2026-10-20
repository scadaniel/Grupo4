using System.Collections.ObjectModel;
using System.Windows.Input;
using App_v3.Models;
using App_v3.Services;

namespace App_v3.ViewModels
{
    public class EventoCalendarioItem
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = string.Empty; // "Clase" o "Trabajo Práctico"
        public string Descripcion { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string FechaFormateada => Fecha.ToString("dd/MM/yyyy");
    }

    public class CalendarViewModel : BindableObject
    {
        private readonly IDatabaseService _databaseService;

        private string _tipoSeleccionado = "Clase";
        private string _tituloInput = string.Empty;
        private DateTime _fechaInput = DateTime.Today;

        public ObservableCollection<string> TiposEvento { get; set; } = new() { "Clase", "Trabajo Práctico" };
        public ObservableCollection<EventoCalendarioItem> EventosCalendario { get; set; } = new();

        public string TipoSeleccionado
        {
            get => _tipoSeleccionado;
            set { _tipoSeleccionado = value; OnPropertyChanged(); }
        }

        public string TituloInput
        {
            get => _tituloInput;
            set { _tituloInput = value; OnPropertyChanged(); }
        }

        public DateTime FechaInput
        {
            get => _fechaInput;
            set { _fechaInput = value; OnPropertyChanged(); }
        }

        public ICommand GuardarEventoCommand { get; }
        public ICommand CargarEventosCommand { get; }

        public CalendarViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;

            GuardarEventoCommand = new Command(async () => await GuardarEventoAsync());
            CargarEventosCommand = new Command(async () => await CargarEventosAsync());

            // Cargar de SQLite al iniciar
            _ = CargarEventosAsync();
        }

        public async Task CargarEventosAsync()
        {
            try
            {
                EventosCalendario.Clear();

                // 1. Cargar Clases registradas
                var clases = await _databaseService.GetClasesAsync();
                foreach (var c in clases)
                {
                    EventosCalendario.Add(new EventoCalendarioItem
                    {
                        Id = c.Id,
                        Tipo = "Clase",
                        Descripcion = string.IsNullOrWhiteSpace(c.Tema)
                            ? $"{c.NombreMateria} (Comisión {c.Comision})"
                            : $"Clase {c.NumeroClase}: {c.Tema}",
                        Fecha = c.Fecha
                    });
                }

                // 2. Cargar Trabajos Prácticos registrados
                var tps = await _databaseService.GetTrabajosPracticosAsync();
                foreach (var tp in tps)
                {
                    EventosCalendario.Add(new EventoCalendarioItem
                    {
                        Id = tp.Id,
                        Tipo = "Trabajo Práctico",
                        Descripcion = string.IsNullOrWhiteSpace(tp.Descripcion)
                            ? tp.Nombre
                            : $"{tp.Nombre} - {tp.Descripcion}",
                        Fecha = tp.FechaLimite
                    });
                }

                // 3. Ordenar por fecha para mostrar cronológicamente (CA 2.3)
                var eventosOrdenados = EventosCalendario.OrderBy(e => e.Fecha).ToList();
                EventosCalendario.Clear();
                foreach (var ev in eventosOrdenados)
                {
                    EventosCalendario.Add(ev);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Calendar Error]: {ex.Message}");
            }
        }

        private async Task GuardarEventoAsync()
        {
            // CA 2.4: Validación de datos obligatorios
            if (string.IsNullOrWhiteSpace(TipoSeleccionado) || string.IsNullOrWhiteSpace(TituloInput))
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Error de validación", "Por favor, complete todos los campos obligatorios (Tipo y Título/Tema) antes de guardar.", "OK");
                }
                return;
            }

            try
            {
                // CA 2.1 y CA 2.2: Persistencia oficial en SQLite según el tipo de evento
                if (TipoSeleccionado == "Clase")
                {
                    int proximoNumero = (await _databaseService.GetClasesAsync()).Count + 1;
                    var nuevaClase = new Clase
                    {
                        NombreMateria = "Materia General",
                        Comision = "Única",
                        NumeroClase = proximoNumero,
                        Tema = TituloInput.Trim(),
                        Fecha = FechaInput
                    };
                    await _databaseService.SaveClaseAsync(nuevaClase);
                }
                else if (TipoSeleccionado == "Trabajo Práctico")
                {
                    var nuevoTp = new TrabajoPractico
                    {
                        Nombre = TituloInput.Trim(),
                        FechaLimite = FechaInput
                    };
                    await _databaseService.SaveTrabajoPracticoAsync(nuevoTp);
                }

                // Limpiar campos y recargar la lista
                TituloInput = string.Empty;
                await CargarEventosAsync();

                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Éxito", "El evento se ha registrado correctamente en la base de datos.", "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Guardar Evento Error]: {ex.Message}");
            }
        }
    }
}