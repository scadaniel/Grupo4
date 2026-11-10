using App_v3.Models;
using App_v3.Services;

namespace App_v3.Views;

[QueryProperty(nameof(ComisionSeleccionada), "Comision")]
public partial class ReporteRiesgoComisionPage : ContentPage
{
    private readonly DatabaseService _databaseService;

    private string _comisionSeleccionada = string.Empty;
    public string ComisionSeleccionada
    {
        get => _comisionSeleccionada;
        set
        {
            _comisionSeleccionada = value;
            Title = $"Comisión: {_comisionSeleccionada}";
        }
    }

    public ReporteRiesgoComisionPage(DatabaseService databaseService)
    {
        InitializeComponent();
        _databaseService = databaseService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarDatosAsync();
    }

    // 🌟 MÉTODO PARA EL BOTÓN AZUL "← VOLVER"
    private async void OnVolverClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async Task CargarDatosAsync()
    {
        if (string.IsNullOrEmpty(ComisionSeleccionada)) return;

        var estudiantes = await _databaseService.GetEstudiantesAsync();
        var filtradosEstudiantes = estudiantes
            .Where(e => e.Comision.Equals(ComisionSeleccionada, StringComparison.OrdinalIgnoreCase))
            .ToList();

        TotalAlumnosLabel.Text = filtradosEstudiantes.Count.ToString();
        int riesgoRojo = filtradosEstudiantes.Count(e => e.TpsNoEntregados >= 3 || e.Inasistencias >= 5);
        RiesgoRojoLabel.Text = riesgoRojo.ToString();

        // Obtenemos las intervenciones/llamadas registradas para enriquecer el reporte
        List<IntervencionContacto> todasIntervenciones = new();
        try
        {
            todasIntervenciones = await _databaseService.GetIntervencionesAsync();
        }
        catch
        {
            // Manejo silencioso si la consulta no devuelve datos
        }

        // Creamos una lista enriquecida para mostrar en la interfaz
        var listaDetalle = new List<EstudianteModelDto>();
        foreach (var est in filtradosEstudiantes)
        {
            bool enRiesgo = est.TpsNoEntregados >= 3 || est.Inasistencias >= 5;

            // Buscar la última intervención o llamada registrada para este estudiante
            var ultimaIntervencion = todasIntervenciones
                .Where(i => i.EstudianteId == est.Id)
                .OrderByDescending(i => i.Fecha)
                .FirstOrDefault();

            string infoContacto = "Sin contactos o llamadas registradas.";
            if (ultimaIntervencion != null)
            {
                infoContacto = $"Último contacto ({ultimaIntervencion.Medio} - {ultimaIntervencion.Fecha:dd/MM/yyyy}): {ultimaIntervencion.Resultado}";
            }

            listaDetalle.Add(new EstudianteModelDto
            {
                NombreCompleto = est.NombreCompleto,
                Legajo = est.Legajo,
                Email = est.Email,
                Inasistencias = est.Inasistencias,
                TpsNoEntregados = est.TpsNoEntregados,
                EstaEnRiesgo = enRiesgo,
                ColorRiesgo = enRiesgo ? "#EF4444" : "#22C55E",
                TextoUltimaIntervencion = infoContacto
            });
        }

        EstudiantesCollection.ItemsSource = listaDetalle;
    }

    private async void OnEnviarEmailClicked(object sender, EventArgs e)
    {
        try
        {
            var filtrados = EstudiantesCollection.ItemsSource as IEnumerable<EstudianteModelDto> ?? new List<EstudianteModelDto>();
            int total = filtrados.Count();
            int rojos = filtrados.Count(est => est.EstaEnRiesgo);

            var cuerpo = $"INFORME DETALLADO DE RIESGO ACADÉMICO\n" +
                         $"Comisión: {ComisionSeleccionada}\n" +
                         $"Alumnos en Riesgo (Rojo): {rojos} / Total: {total}\n\n" +
                         $"----------------------------------------\n";

            foreach (var est in filtrados)
            {
                cuerpo += $"• {est.NombreCompleto} (Legajo: {est.Legajo})\n" +
                          $"  - Inasistencias: {est.Inasistencias} | TPs Pendientes: {est.TpsNoEntregados}\n" +
                          $"  - {est.TextoUltimaIntervencion}\n" +
                          $"----------------------------------------\n";
            }

            // Copia automáticamente el texto listo al portapapeles de la PC
            await Clipboard.Default.SetTextAsync(cuerpo);

            await DisplayAlert("¡Copiado al Portapapeles!",
                "El informe completo se ha copiado con éxito.\n\nYa puedes ir a tu navegador (Gmail u Outlook web), redactar un correo nuevo y presionar Pegar (Ctrl + V).",
                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo copiar el texto: {ex.Message}", "OK");
        }
    }
}

// Modelo auxiliar DTO para mostrar toda la info junta en pantalla
public class EstudianteModelDto
{
    public string NombreCompleto { get; set; } = string.Empty;
    public string Legajo { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Inasistencias { get; set; }
    public int TpsNoEntregados { get; set; }
    public bool EstaEnRiesgo { get; set; }
    public string ColorRiesgo { get; set; } = "#22C55E";
    public string TextoUltimaIntervencion { get; set; } = string.Empty;
}