using App_v3.Models;
using App_v3.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace App_v3.ViewModels
{
    [QueryProperty(nameof(ComisionSeleccionada), "Comision")]
    public partial class ReporteRiesgoComisionViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private string comisionSeleccionada = string.Empty;

        [ObservableProperty]
        private int totalAlumnos;

        [ObservableProperty]
        private int alumnosEnRiesgoCount;

        [ObservableProperty]
        private string estadoRiesgoTexto = string.Empty;

        [ObservableProperty]
        private string emailDestinatario = string.Empty;

        public ObservableCollection<Estudiante> EstudiantesComision { get; } = new();

        // Comando para volver al menú anterior
        public ICommand VolverAlMenuCommand { get; }

        public ReporteRiesgoComisionViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            VolverAlMenuCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        }

        [RelayCommand]
        public async Task CargarDatosAsync()
        {
            if (string.IsNullOrEmpty(ComisionSeleccionada)) return;

            // 🌟 Usamos la función 100% nueva y exclusiva para este módulo
            var filtrados = await _databaseService.ObtenerEstudiantesParaReporteRiesgoAsync(ComisionSeleccionada);

            TotalAlumnos = filtrados?.Count ?? 0;

            EstudiantesComision.Clear();
            if (filtrados != null)
            {
                foreach (var est in filtrados)
                {
                    EstudiantesComision.Add(est);
                }
            }

            ActualizarContadoresYTexto();
        }

        private void ActualizarContadoresYTexto()
        {
            if (TotalAlumnos == 0)
            {
                AlumnosEnRiesgoCount = 0;
                EstadoRiesgoTexto = "No hay suficiente información";
                return;
            }

            int enRiesgoCount = 0;
            int sinDatosCount = 0;

            foreach (var est in EstudiantesComision)
            {
                // Validación de información suficiente
                bool tieneDatos = est.Inasistencias >= 0 && est.TpsNoEntregados >= 0;
                if (!tieneDatos)
                {
                    sinDatosCount++;
                    continue;
                }

                // Criterio de riesgo (TpsNoEntregados >= 3 o Inasistencias >= 5)
                if (est.TpsNoEntregados >= 3 || est.Inasistencias >= 5)
                {
                    enRiesgoCount++;
                }
            }

            AlumnosEnRiesgoCount = enRiesgoCount;

            // Aplicación estricta de las reglas de texto solicitadas
            if (sinDatosCount == TotalAlumnos)
            {
                EstadoRiesgoTexto = "No hay suficiente información";
            }
            else if (enRiesgoCount == 0)
            {
                EstadoRiesgoTexto = $"No hay estudiantes en riesgo / {TotalAlumnos} total de estudiantes";
            }
            else
            {
                EstadoRiesgoTexto = $"{enRiesgoCount} estudiantes en riesgo / {TotalAlumnos} total de estudiantes";
            }
        }

        [RelayCommand]
        public async Task EnviarReporteEmailAsync()
        {
            if (string.IsNullOrWhiteSpace(EmailDestinatario))
            {
                await Shell.Current.DisplayAlert("Aviso", "Por favor, ingresa un correo electrónico válido.", "OK");
                return;
            }

            try
            {
                ActualizarContadoresYTexto();

                var cuerpoMensaje = $"========================================\n" +
                                    $" INFORME DETALLADO DE RIESGO ACADÉMICO\n" +
                                    $" Comisión: {ComisionSeleccionada}\n" +
                                    $"========================================\n\n" +
                                    $"• Estado General: {EstadoRiesgoTexto}\n" +
                                    $"• Alumnos en Riesgo: {AlumnosEnRiesgoCount}\n" +
                                    $"• Total de Alumnos en la Comisión: {TotalAlumnos}\n\n" +
                                    $"----------------------------------------\n" +
                                    $" DETALLE DE ESTUDIANTES:\n" +
                                    $"----------------------------------------\n";

                if (EstudiantesComision.Count == 0)
                {
                    cuerpoMensaje += "No hay estudiantes registrados en esta comisión.\n";
                }
                else
                {
                    foreach (var est in EstudiantesComision)
                    {
                        cuerpoMensaje += $"- Nombre: {est.NombreCompleto}\n" +
                                         $"  Legajo: {est.Legajo} | Email: {est.Email}\n" +
                                         $"  Inasistencias: {est.Inasistencias} | TPs Pendientes: {est.TpsNoEntregados}\n" +
                                         $"----------------------------------------\n";
                    }
                }

                cuerpoMensaje += "\nGenerado automáticamente por el Sistema de Alerta Temprana.";

                if (Email.Default.IsComposeSupported)
                {
                    var message = new EmailMessage
                    {
                        Subject = $"Informe de Riesgo Académico - Comisión {ComisionSeleccionada}",
                        Body = cuerpoMensaje,
                        To = new List<string> { EmailDestinatario.Trim() }
                    };

                    await Email.Default.ComposeAsync(message);
                }
                else
                {
                    await Shell.Current.DisplayAlert("Aviso", "El envío de correos no está soportado en este dispositivo.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"No se pudo generar el correo: {ex.Message}", "OK");
            }
        }
    }
}