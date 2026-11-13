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

            if (!string.IsNullOrWhiteSpace(_comisionSeleccionada))
            {
                _ = CargarDatosAsync();
            }
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

        if (!string.IsNullOrEmpty(ComisionSeleccionada))
        {
            await CargarDatosAsync();
        }
    }

    private async void OnVolverClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async Task CargarDatosAsync()
    {
        if (string.IsNullOrEmpty(ComisionSeleccionada)) return;

        // 🌟 Llamamos ÚNICAMENTE a la función 100% nueva y aislada
        var filtradosEstudiantes = await _databaseService.ObtenerEstudiantesParaReporteRiesgoExclusivoAsync(ComisionSeleccionada);

        if (filtradosEstudiantes == null)
        {
            filtradosEstudiantes = new List<Estudiante>();
        }

        int totalAlumnos = filtradosEstudiantes.Count;
        TotalAlumnosLabel.Text = totalAlumnos.ToString();

        int riesgoRojo = 0;

        List<IntervencionContacto> todasIntervenciones = new();
        try
        {
            todasIntervenciones = await _databaseService.GetIntervencionesAsync();
        }
        catch { }

        var listaDetalle = new List<EstudianteModelDto>();

        foreach (var est in filtradosEstudiantes)
        {
            bool enRiesgo = est.TpsNoEntregados >= 3 || est.Inasistencias >= 5;
            if (enRiesgo)
            {
                riesgoRojo++;
            }

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
                NombreCompleto = est.NombreCompleto ?? "Sin Nombre",
                Legajo = est.Legajo ?? "S/N",
                Email = est.Email ?? "",
                Inasistencias = est.Inasistencias,
                TpsNoEntregados = est.TpsNoEntregados,
                EstaEnRiesgo = enRiesgo,
                ColorRiesgo = enRiesgo ? "#EF4444" : "#22C55E",
                TextoUltimaIntervencion = infoContacto
            });
        }

        RiesgoRojoLabel.Text = riesgoRojo.ToString();
        EstudiantesCollection.ItemsSource = listaDetalle;

        // Validación estricta de textos solicitados
        if (totalAlumnos == 0)
        {
            EstadoRiesgoLabel.Text = "No hay suficiente información";
        }
        else if (riesgoRojo == 0)
        {
            EstadoRiesgoLabel.Text = $"No hay estudiantes en riesgo / {totalAlumnos} total de estudiantes";
        }
        else
        {
            EstadoRiesgoLabel.Text = $"{riesgoRojo} estudiantes en riesgo / {totalAlumnos} total de estudiantes";
        }
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