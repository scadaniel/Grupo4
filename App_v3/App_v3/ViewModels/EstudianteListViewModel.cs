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
        public ObservableCollection<Comision> ListaComisiones { get; set; } = new();

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
                    // Al cambiar de comisión, recalculamos y filtramos los alumnos automáticamente
                    _ = FiltrarYCalcularRiesgoPorComisionAsync();
                }
            }
        }

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

        public ICommand CargarComisionesCommand { get; }
        public ICommand ResetDatabaseCommand { get; }

        public EstudianteListViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;

            CargarComisionesCommand = new Command(async () => await CargarComisionesAsync());
            ResetDatabaseCommand = new Command(async () => await BorrarBaseDeDatosAsync());

            // Inicializamos cargando las comisiones
            _ = InicializarVistaAsync();
        }

        private async Task InicializarVistaAsync()
        {
            await CargarComisionesAsync();

            // Si hay comisiones, seleccionamos la primera por defecto para que la vista no arranque vacía
            if (ListaComisiones.Count > 0 && ComisionSeleccionada == null)
            {
                ComisionSeleccionada = ListaComisiones[0];
            }
        }

        public async Task CargarComisionesAsync()
        {
            try
            {
                ListaComisiones.Clear();
                var comisiones = await _databaseService.GetComisionesAsync();
                if (comisiones != null)
                {
                    foreach (var com in comisiones)
                    {
                        ListaComisiones.Add(com);
                    }
                }
                System.Diagnostics.Debug.WriteLine($"[Comisiones Cargadas]: {ListaComisiones.Count} comisiones encontradas.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Error Comisiones]: {ex.Message}");
            }
        }

        /// <summary>
        /// Método de compatibilidad requerido por el code-behind de la vista (EstudianteListPage.xaml.cs).
        /// Redirige al flujo de filtrado y cálculo de riesgo por comisión.
        /// </summary>
        public async Task CargarEstudiantesAsync()
        {
            await CargarComisionesAsync();
            await FiltrarYCalcularRiesgoPorComisionAsync();
        }

        /// <summary>
        /// Filtra los estudiantes por la comisión seleccionada utilizando el servicio oficial 
        /// y evalúa su riesgo usando los umbrales específicos de esa comisión, ordenándolos por gravedad.
        /// </summary>
        public async Task FiltrarYCalcularRiesgoPorComisionAsync()
        {
            try
            {
                Estudiantes.Clear();

                if (ComisionSeleccionada == null || string.IsNullOrWhiteSpace(ComisionSeleccionada.Nombre))
                {
                    System.Diagnostics.Debug.WriteLine("[Riesgo]: No hay comisión seleccionada o su nombre es nulo.");
                    return;
                }

                string nombreComision = ComisionSeleccionada.Nombre.Trim();
                System.Diagnostics.Debug.WriteLine($"[Riesgo]: Buscando alumnos para la comisión '{nombreComision}'...");

                // Obtenemos los estudiantes usando el método oficial del servicio (igual que TomaAsistenciaViewModel)
                var alumnosDeComision = await _databaseService.GetEstudiantesPorComisionAsync(nombreComision);

                if (alumnosDeComision == null || !alumnosDeComision.Any())
                {
                    System.Diagnostics.Debug.WriteLine($"[Riesgo]: No se encontraron alumnos asociados a la comisión '{nombreComision}'.");
                    return;
                }

                var listaTemporal = new List<EstudianteItemViewModel>();

                foreach (var est in alumnosDeComision)
                {
                    int inasistencias = est.Inasistencias;
                    int tpsNoEntregados = est.TpsNoEntregados;

                    // Evaluamos el riesgo utilizando los umbrales personalizados de la comisión actual
                    var resultadoRiesgo = CalcularRiesgoPersonalizado(inasistencias, tpsNoEntregados, ComisionSeleccionada);

                    var itemVm = new EstudianteItemViewModel(
                        est,
                        resultadoRiesgo.NivelEnum,
                        resultadoRiesgo.ColorHex,
                        resultadoRiesgo.Diagnostico
                    );

                    listaTemporal.Add(itemVm);
                }

                // Ordenar de mayor gravedad a menor gravedad (Alto -> Medio -> Bajo) y luego alfabéticamente
                var listaOrdenada = listaTemporal
                    .OrderByDescending(x => ObtenerPesoGravedad(x.NivelRiesgoEnum))
                    .ThenBy(x => x.NombreCompleto);

                foreach (var item in listaOrdenada)
                {
                    Estudiantes.Add(item);
                }

                System.Diagnostics.Debug.WriteLine($"[Riesgo]: Se cargaron y evaluaron {Estudiantes.Count} estudiantes con éxito.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Error Filtrado Riesgo]: {ex.Message}");
            }
        }

        /// <summary>
        /// Asigna un peso numérico al nivel de riesgo para ordenarlos de mayor a menor gravedad.
        /// </summary>
        private int ObtenerPesoGravedad(NivelRiesgo nivelRiesgo)
        {
            return nivelRiesgo switch
            {
                NivelRiesgo.Alto => 3,
                NivelRiesgo.Medio => 2,
                NivelRiesgo.Bajo => 1,
                _ => 0
            };
        }

        /// <summary>
        /// Evalúa el riesgo comparando las inasistencias y TPs del alumno con los umbrales de la comisión.
        /// </summary>
        private (string NivelTexto, NivelRiesgo NivelEnum, string ColorHex, string Diagnostico) CalcularRiesgoPersonalizado(int inasistencias, int tpsNoEntregados, Comision comision)
        {
            int limiteInasBajo = comision.InasistenciasBajo;
            int limiteInasMedio = comision.InasistenciasMedio;
            int limiteTpsBajo = comision.TpsBajo;
            int limiteTpsMedio = comision.TpsMedio;

            bool inasAlto = inasistencias > limiteInasMedio;
            bool inasMedio = inasistencias > limiteInasBajo && inasistencias <= limiteInasMedio;

            bool tpsAlto = tpsNoEntregados > limiteTpsMedio;
            bool tpsMedio = tpsNoEntregados > limiteTpsBajo && tpsNoEntregados <= limiteTpsMedio;

            string nivelTexto;
            NivelRiesgo nivelEnum;
            string colorHex;
            List<string> razones = new();

            if (inasAlto || tpsAlto)
            {
                nivelTexto = "ALTO";
                nivelEnum = NivelRiesgo.Alto;
                colorHex = "#D32F2F"; // Rojo fuerte de alerta

                if (inasAlto) razones.Add($"Inasistencias críticas ({inasistencias} > límite medio {limiteInasMedio})");
                if (tpsAlto) razones.Add($"Exceso grave de TPs sin entregar ({tpsNoEntregados} > límite medio {limiteTpsMedio})");
            }
            else if (inasMedio || tpsMedio)
            {
                nivelTexto = "MEDIO";
                nivelEnum = NivelRiesgo.Medio;
                colorHex = "#F57C00"; // Amarillo / Naranja de precaución

                if (inasMedio) razones.Add($"Inasistencias moderadas ({inasistencias} en rango {limiteInasBajo}-{limiteInasMedio})");
                if (tpsMedio) razones.Add($"TPs pendientes moderados ({tpsNoEntregados} en rango {limiteTpsBajo}-{limiteTpsMedio})");
            }
            else
            {
                nivelTexto = "BAJO";
                nivelEnum = NivelRiesgo.Bajo;
                colorHex = "#388E3C"; // Verde institucional de normalidad
                razones.Add("Cumple con los parámetros normales de asistencia y entrega de TPs.");
            }

            string diagnostico = string.Join(" • ", razones);
            return (nivelTexto, nivelEnum, colorHex, diagnostico);
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