using App_v3.Models;
using App_v3.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace App_v3.ViewModels;

[QueryProperty(nameof(ComisionSeleccionada), "Comision")]
public partial class ReporteRiesgoComisionViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;

    [ObservableProperty]
    private string comisionSeleccionada = string.Empty;

    [ObservableProperty]
    private int totalAlumnos;

    [ObservableProperty]
    private int alumnosEnRiesgoRojo;

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

        var todosLosEstudiantes = await _databaseService.GetEstudiantesAsync();
        var filtrados = todosLosEstudiantes
            .Where(e => e.Comision.Equals(ComisionSeleccionada, StringComparison.OrdinalIgnoreCase))
            .ToList();

        TotalAlumnos = filtrados.Count;
        AlumnosEnRiesgoRojo = filtrados.Count(e => e.TpsNoEntregados >= 3 || e.Inasistencias >= 5);

        EstudiantesComision.Clear();
        foreach (var est in filtrados)
        {
            EstudiantesComision.Add(est);
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
            var cuerpoMensaje = $"========================================\n" +
                                $" INFORME DETALLADO DE RIESGO ACADÉMICO\n" +
                                $" Comisión: {ComisionSeleccionada}\n" +
                                $"========================================\n\n" +
                                $"• Alumnos en Riesgo (Rojo): {AlumnosEnRiesgoRojo}\n" +
                                $"• Total de Alumnos en la Comisión: {TotalAlumnos}\n\n" +
                                $"----------------------------------------\n" +
                                $" DETALLE DE ESTUDIANTES:\n" +
                                $"----------------------------------------\n";

            foreach (var est in EstudiantesComision)
            {
                cuerpoMensaje += $"- Nombre: {est.NombreCompleto}\n" +
                                 $"  Legajo: {est.Legajo} | Email: {est.Email}\n" +
                                 $"  Inasistencias: {est.Inasistencias} | TPs Pendientes: {est.TpsNoEntregados}\n" +
                                 $"----------------------------------------\n";
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