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

            _ = InicializarVistaAsync();
        }

        private async Task InicializarVistaAsync()
        {
            await CargarComisionesAsync();

            if (ListaComisiones.Count > 0 && ComisionSeleccionada == null)
            {
                ComisionSeleccionada = ListaComisiones[0];
            }
        }

        public async Task CargarComisionesAsync()
        {
            try
            {
                var comisiones = await _databaseService.GetComisionesAsync();

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    ListaComisiones.Clear();
                    if (comisiones != null)
                    {
                        var comisionesUnicas = comisiones
                            .Where(c => !string.IsNullOrWhiteSpace(c.Nombre))
                            .GroupBy(c => c.Nombre.Trim())
                            .Select(g => g.First())
                            .ToList();

                        foreach (var com in comisionesUnicas)
                        {
                            ListaComisiones.Add(com);
                        }
                    }
                });

                System.Diagnostics.Debug.WriteLine($"[Comisiones Cargadas]: {ListaComisiones.Count} comisiones únicas encontradas.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Error Comisiones]: {ex.Message}");
            }
        }

        public async Task CargarEstudiantesAsync()
        {
            await CargarComisionesAsync();
            await FiltrarYCalcularRiesgoPorComisionAsync();
        }

        public async Task FiltrarYCalcularRiesgoPorComisionAsync()
        {
            try
            {
                if (ComisionSeleccionada == null || string.IsNullOrWhiteSpace(ComisionSeleccionada.Nombre))
                {
                    System.Diagnostics.Debug.WriteLine("[Riesgo]: No hay comisión seleccionada o su nombre es nulo.");
                    await MainThread.InvokeOnMainThreadAsync(() => Estudiantes.Clear());
                    return;
                }

                string nombreComision = ComisionSeleccionada.Nombre.Trim();
                System.Diagnostics.Debug.WriteLine($"[Riesgo]: Buscando alumnos para la comisión '{nombreComision}'...");

                var alumnosDeComision = await _databaseService.GetEstudiantesPorComisionAsync(nombreComision);

                var listaTemporal = new List<EstudianteItemViewModel>();

                if (alumnosDeComision != null)
                {
                    foreach (var est in alumnosDeComision)
                    {
                        int inasistencias = est.Inasistencias;
                        int tpsNoEntregados = est.TpsNoEntregados;

                        var resultadoRiesgo = CalcularRiesgoPersonalizado(inasistencias, tpsNoEntregados, ComisionSeleccionada);

                        var itemVm = new EstudianteItemViewModel(
                            est,
                            resultadoRiesgo.NivelEnum,
                            resultadoRiesgo.ColorHex,
                            resultadoRiesgo.Diagnostico
                        );

                        listaTemporal.Add(itemVm);
                    }
                }

                var listaOrdenada = listaTemporal
                    .OrderByDescending(x => ObtenerPesoGravedad(x.NivelRiesgoEnum))
                    .ThenBy(x => x.NombreCompleto)
                    .ToList();

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Estudiantes.Clear();
                    foreach (var item in listaOrdenada)
                    {
                        Estudiantes.Add(item);
                    }
                });

                System.Diagnostics.Debug.WriteLine($"[Riesgo]: Se cargaron y evaluaron {Estudiantes.Count} estudiantes con éxito.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Error Filtrado Riesgo]: {ex.Message}");
            }
        }

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

        private (string NivelTexto, NivelRiesgo NivelEnum, string ColorHex, string Diagnostico) CalcularRiesgoPersonalizado(int inasistencias, int tpsNoEntregados, Comision? comision)
        {
            // Blindaje contra nulos por si el objeto comisión llega vacío al inicializar
            int limiteInasBajo = comision?.InasistenciasBajo ?? 3;
            int limiteInasMedio = comision?.InasistenciasMedio ?? 6;
            int limiteTpsBajo = comision?.TpsBajo ?? 2;
            int limiteTpsMedio = comision?.TpsMedio ?? 4;

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
                colorHex = "#D32F2F";

                if (inasAlto) razones.Add($"Inasistencias críticas ({inasistencias} > límite medio {limiteInasMedio})");
                if (tpsAlto) razones.Add($"Exceso grave de TPs sin entregar ({tpsNoEntregados} > límite medio {limiteTpsMedio})");
            }
            else if (inasMedio || tpsMedio)
            {
                nivelTexto = "MEDIO";
                nivelEnum = NivelRiesgo.Medio;
                colorHex = "#F57C00";

                if (inasMedio) razones.Add($"Inasistencias moderadas ({inasistencias} en rango {limiteInasBajo}-{limiteInasMedio})");
                if (tpsMedio) razones.Add($"TPs pendientes moderados ({tpsNoEntregados} en rango {limiteTpsBajo}-{limiteTpsMedio})");
            }
            else
            {
                nivelTexto = "BAJO";
                nivelEnum = NivelRiesgo.Bajo;
                colorHex = "#388E3C";
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
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Estudiantes.Clear();
                    ListaComisiones.Clear();
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Reset Error]: {ex.Message}");
            }
        }
    }
}