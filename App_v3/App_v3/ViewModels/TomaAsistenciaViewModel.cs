using System.Collections.ObjectModel;
using System.Windows.Input;
using App_v3.Models;
using App_v3.Services;

namespace App_v3.ViewModels
{
    public class AsistenciaItemViewModel : BindableObject
    {
        public int EstudianteId { get; set; }
        public int AsistenciaId { get; set; }
        public string NombreEstudiante { get; set; } = string.Empty;
        public string Legajo { get; set; } = string.Empty;

        private bool _presente = true;
        public bool Presente
        {
            get => _presente;
            set { _presente = value; OnPropertyChanged(); }
        }

        private bool _justificada = false;
        public bool Justificada
        {
            get => _justificada;
            set { _justificada = value; OnPropertyChanged(); }
        }
    }

    public class TomaAsistenciaViewModel : BindableObject
    {
        private readonly DatabaseService _databaseService;

        public ObservableCollection<Clase> Clases { get; set; } = new();
        public ObservableCollection<AsistenciaItemViewModel> ListaAlumnos { get; set; } = new();

        private Clase? _claseSeleccionada;
        public Clase? ClaseSeleccionada
        {
            get => _claseSeleccionada;
            set
            {
                if (_claseSeleccionada != value)
                {
                    _claseSeleccionada = value;
                    OnPropertyChanged();
                    if (value != null)
                    {
                        // Carga los alumnos específicos de esta clase/comisión al seleccionarla
                        _ = CargarListaAlumnosPorClaseAsync(value);
                    }
                }
            }
        }

        public ICommand GuardarAsistenciaCommand { get; }
        public ICommand CargarClasesCommand { get; }

        public TomaAsistenciaViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            GuardarAsistenciaCommand = new Command(async () => await GuardarAsistenciaAsync());
            CargarClasesCommand = new Command(async () => await CargarClasesAsync());

            _ = CargarClasesAsync();
        }

        public async Task CargarClasesAsync()
        {
            try
            {
                var listaClases = await _databaseService.GetClasesAsync();
                Clases.Clear();

                foreach (var c in listaClases.OrderBy(x => x.Fecha))
                {
                    Clases.Add(c);
                }

                // Si hay clases, seleccionamos la primera por defecto para disparar la carga
                if (Clases.Count > 0 && ClaseSeleccionada == null)
                {
                    ClaseSeleccionada = Clases.First();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Error cargando clases]: {ex.Message}");
            }
        }

        public async Task CargarListaAlumnosPorClaseAsync(Clase clase)
        {
            ListaAlumnos.Clear();

            // Obtenemos los estudiantes de la comisión correspondiente a la clase seleccionada
            var estudiantes = await _databaseService.GetEstudiantesPorComisionAsync(clase.Comision);
            var asistenciasExistentes = await _databaseService.GetAsistenciasPorClaseAsync(clase.Id);

            foreach (var est in estudiantes)
            {
                var astExistente = asistenciasExistentes.FirstOrDefault(a => a.EstudianteId == est.Id);

                ListaAlumnos.Add(new AsistenciaItemViewModel
                {
                    EstudianteId = est.Id,
                    AsistenciaId = astExistente?.Id ?? 0,
                    NombreEstudiante = est.NombreCompleto,
                    Legajo = est.Legajo,
                    Presente = astExistente?.Presente ?? true,
                    Justificada = astExistente?.Justificada ?? false
                });
            }
        }

        public async Task GuardarAsistenciaAsync()
        {
            if (ClaseSeleccionada == null)
            {
                if (Shell.Current != null)
                    await Shell.Current.DisplayAlert("Atención", "Debe seleccionar una clase antes de guardar la asistencia.", "OK");
                return;
            }

            var listaAGuardar = ListaAlumnos.Select(item => new Asistencia
            {
                Id = item.AsistenciaId,
                ClaseId = ClaseSeleccionada.Id,
                EstudianteId = item.EstudianteId,
                Presente = item.Presente,
                Justificada = item.Justificada,
                Fecha = ClaseSeleccionada.Fecha
            }).ToList();

            await _databaseService.GuardarListaAsistenciasAsync(ClaseSeleccionada.Id, listaAGuardar);

            if (Shell.Current != null)
            {
                await Shell.Current.DisplayAlert("Asistencia Guardada", "Se registró la asistencia correctamente y se actualizaron los indicadores de riesgo.", "OK");
            }
        }
    }
}