using System.Collections.ObjectModel;
using System.Windows.Input;
using App_v3.Models;
using App_v3.Services;
using App_v3.Views;

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

        private string _textoBusqueda = string.Empty;
        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set
            {
                if (_textoBusqueda != value)
                {
                    _textoBusqueda = value;
                    OnPropertyChanged();
                    _ = FiltrarYCalcularRiesgoPorComisionAsync();
                }
            }
        }

        private bool _soloNoContactadosSemana;
        public bool SoloNoContactadosSemana
        {
            get => _soloNoContactadosSemana;
            set
            {
                if (_soloNoContactadosSemana != value)
                {
                    _soloNoContactadosSemana = value;
                    OnPropertyChanged();
                    _ = FiltrarYCalcularRiesgoPorComisionAsync();
                }
            }
        }

        public ICommand CargarComisionesCommand { get; }
        public ICommand ResetDatabaseCommand { get; }
        public ICommand RegistrarContactoAlumnoCommand { get; }

        public EstudianteListViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;

            CargarComisionesCommand = new Command(async () => await CargarComisionesAsync());
            ResetDatabaseCommand = new Command(async () => await BorrarBaseDeDatosAsync());
            RegistrarContactoAlumnoCommand = new Command<EstudianteItemViewModel>(async (item) => await IrARegistrarContactoAsync(item));

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
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Error Comisiones]: {ex.Message}");
            }
        }

        public async Task FiltrarYCalcularRiesgoPorComisionAsync()
        {
            try
            {
                if (ComisionSeleccionada == null || string.IsNullOrWhiteSpace(ComisionSeleccionada.Nombre))
                {
                    await MainThread.InvokeOnMainThreadAsync(() => Estudiantes.Clear());
                    return;
                }

                string nombreComision = ComisionSeleccionada.Nombre.Trim();

                // 🌟 Llamada a la nueva función exclusiva de riesgo en DatabaseService
                var alumnosDeComision = await _databaseService.GetEstudiantesParaRiesgoExclusivoAsync(nombreComision);

                var listaTemporal = new List<EstudianteItemViewModel>();

                if (alumnosDeComision != null)
                {
                    foreach (var est in alumnosDeComision)
                    {
                        // 🔍 Filtro por búsqueda rápida de Nombre, Apellido, Legajo o DNI
                        if (!string.IsNullOrWhiteSpace(TextoBusqueda))
                        {
                            string filtro = TextoBusqueda.Trim().ToLower();
                            bool coincideNombre = est.NombreCompleto?.ToLower().Contains(filtro) ?? false;
                            bool coincideDni = est.Dni?.ToLower().Contains(filtro) ?? false;
                            bool coincideLegajo = est.Legajo?.ToLower().Contains(filtro) ?? false;

                            if (!coincideNombre && !coincideDni && !coincideLegajo)
                            {
                                continue;
                            }
                        }

                        int inasistencias = est.Inasistencias;
                        int tpsNoEntregados = est.TpsNoEntregados;

                        var resultadoRiesgo = CalcularRiesgoPersonalizado(inasistencias, tpsNoEntregados, ComisionSeleccionada);

                        // Consultar el último registro de contacto para el alumno (H11)
                        var ultimoContacto = await _databaseService.GetUltimoContactoEstudianteAsync(est.Id);

                        bool contactado = ultimoContacto != null;
                        string textoContacto = contactado
                            ? $"📞 Contactado el {ultimoContacto.Fecha:dd/MM} ({ultimoContacto.Medio})"
                            : "Sin contacto registrado";

                        // Aplicar el filtro CA 11.2 (No contactados en los últimos 7 días)
                        if (SoloNoContactadosSemana)
                        {
                            bool contactadoEstaSemana = ultimoContacto != null && (DateTime.Today - ultimoContacto.Fecha).TotalDays <= 7;
                            if (contactadoEstaSemana)
                            {
                                continue;
                            }
                        }

                        var itemVm = new EstudianteItemViewModel(
                            est,
                            resultadoRiesgo.NivelEnum,
                            resultadoRiesgo.ColorHex,
                            resultadoRiesgo.Diagnostico
                        )
                        {
                            FueContactado = contactado,
                            TextoEstadoContacto = textoContacto
                        };

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
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Error Filtrado Riesgo]: {ex.Message}");
            }
        }

        private async Task IrARegistrarContactoAsync(EstudianteItemViewModel? item)
        {
            if (item == null || item.Estudiante == null) return;

            var parameters = new Dictionary<string, object>
            {
                { "EstudianteId", item.Estudiante.Id }
            };

            await Shell.Current.GoToAsync(nameof(RegistrarIntervencionPage), parameters);
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