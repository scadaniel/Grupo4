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

        public ObservableCollection<TrabajoPractico> TrabajosPracticos { get; set; } = new();
        public ObservableCollection<EntregaTpItemViewModel> ListaAlumnos { get; set; } = new();

        private TrabajoPractico? _tpSeleccionado;
        public TrabajoPractico? TpSeleccionado
        {
            get => _tpSeleccionado;
            set
            {
                _tpSeleccionado = value;
                OnPropertyChanged();
                if (value != null)
                {
                    _ = CargarListaAlumnosAsync(value.Id);
                }
            }
        }

        public ICommand GuardarEntregasCommand { get; }

        public RegistroEntregasViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            GuardarEntregasCommand = new Command(async () => await GuardarEntregasAsync());

            _ = CargarTrabajosPracticosAsync();
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

        public async Task CargarListaAlumnosAsync(int trabajoPracticoId)
        {
            ListaAlumnos.Clear();
            var estudiantes = await _databaseService.GetEstudiantesAsync();
            var entregasExistentes = await _databaseService.GetEntregasPorTpAsync(trabajoPracticoId);

            foreach (var est in estudiantes.Where(e => e.Activo))
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