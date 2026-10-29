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

        private bool _entregado = false;
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
                _claseSeleccionada = value;
                OnPropertyChanged();
                _ = CargarListaAlumnosAsync();
            }
        }

        private TrabajoPractico? _tpSeleccionado;
        public TrabajoPractico? TpSeleccionado
        {
            get => _tpSeleccionado;
            set
            {
                _tpSeleccionado = value;
                OnPropertyChanged();
                _ = CargarListaAlumnosAsync();
            }
        }

        public ICommand GuardarEntregasCommand { get; }

        public RegistroEntregasViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            GuardarEntregasCommand = new Command(async () => await GuardarEntregasAsync());

            _ = InicializarDatosAsync();
        }

        private async Task InicializarDatosAsync()
        {
            await CargarClasesAsync();
            await CargarTrabajosPracticosAsync();
        }

        public async Task CargarClasesAsync()
        {
            var clases = await _databaseService.GetClasesAsync();
            Clases.Clear();
            foreach (var c in clases)
            {
                Clases.Add(c);
            }

            if (Clases.Count > 0)
            {
                ClaseSeleccionada = Clases.First();
            }
        }

        public async Task CargarTrabajosPracticosAsync()
        {
            var tps = await _databaseService.GetTrabajosPracticosAsync();
            TrabajosPracticos.Clear();
            foreach (var tp in tps)
            {
                TrabajosPracticos.Add(tp);
            }

            if (TrabajosPracticos.Count > 0)
            {
                TpSeleccionado = TrabajosPracticos.First();
            }
        }

        public async Task CargarListaAlumnosAsync()
        {
            ListaAlumnos.Clear();

            if (ClaseSeleccionada == null || TpSeleccionado == null)
                return;

            // Filtramos y traemos únicamente los alumnos inscriptos a la clase seleccionada
            var estudiantes = await _databaseService.GetEstudiantesPorClaseIdAsync(ClaseSeleccionada.Id);
            var entregasExistentes = await _databaseService.GetEntregasPorTpAsync(TpSeleccionado.Id);

            foreach (var est in estudiantes)
            {
                var entregaExistente = entregasExistentes.FirstOrDefault(e => e.EstudianteId == est.Id);

                ListaAlumnos.Add(new EntregaTpItemViewModel
                {
                    EstudianteId = est.Id,
                    EntregaId = entregaExistente?.Id ?? 0,
                    NombreEstudiante = est.NombreCompleto,
                    Legajo = est.Legajo,
                    Entregado = entregaExistente?.Entregado ?? false
                });
            }
        }

        public async Task GuardarEntregasAsync()
        {
            if (TpSeleccionado == null) return;

            var listaAGuardar = ListaAlumnos.Select(item => new EntregaTp
            {
                Id = item.EntregaId,
                TrabajoPracticoId = TpSeleccionado.Id,
                EstudianteId = item.EstudianteId,
                Entregado = item.Entregado,
                FechaEntrega = item.Entregado ? DateTime.Now : null
            }).ToList();

            await _databaseService.GuardarListaEntregasTpAsync(TpSeleccionado.Id, listaAGuardar);

            if (Shell.Current != null)
            {
                await Shell.Current.DisplayAlert("Entregas Guardadas", "Se registraron las entregas del TP y se reevaluó el indicador de riesgo de los estudiantes.", "OK");
            }
        }
    }
}