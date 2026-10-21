using System.Collections.ObjectModel;
using System.Windows.Input;
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

    /// <summary>
    /// Pantalla del calendario. Solo se ocupa de la pantalla: las reglas (validar, numerar,
    /// guardar y ordenar) viven en CalendarioService, dentro de App_v3.Core, donde se prueban.
    /// </summary>
    public class CalendarViewModel : BindableObject
    {
        private readonly CalendarioService _calendario;

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
            _calendario = CalendarioService.Para(databaseService);

            GuardarEventoCommand = new Command(async () => await GuardarEventoAsync());
            CargarEventosCommand = new Command(async () => await CargarEventosAsync());

            // Cargar de SQLite al iniciar
            _ = CargarEventosAsync();
        }

        // CA 2.3: el servicio devuelve clases y trabajos juntos, ya en orden cronológico
        public async Task CargarEventosAsync()
        {
            try
            {
                var eventos = await _calendario.ListarEventosAsync();

                EventosCalendario.Clear();
                foreach (var evento in eventos)
                {
                    EventosCalendario.Add(new EventoCalendarioItem
                    {
                        Id = evento.Id,
                        Tipo = evento.Tipo,
                        Descripcion = evento.Descripcion,
                        Fecha = evento.Fecha
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Calendar Error]: {ex.Message}");
            }
        }

        // CA 2.1, CA 2.2 y CA 2.4: el servicio valida y guarda; acá solo se muestra el resultado
        private async Task GuardarEventoAsync()
        {
            try
            {
                await _calendario.AgregarEventoAsync(TipoSeleccionado, TituloInput, FechaInput);

                // Limpiar campos y recargar la lista
                TituloInput = string.Empty;
                await CargarEventosAsync();

                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Éxito", "El evento se ha registrado correctamente en la base de datos.", "OK");
                }
            }
            catch (DatosInvalidosException ex)
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Error de validación", ex.Message, "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Guardar Evento Error]: {ex.Message}");
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Error", "No se pudo guardar el evento. Intente nuevamente.", "OK");
                }
            }
        }
    }
}
