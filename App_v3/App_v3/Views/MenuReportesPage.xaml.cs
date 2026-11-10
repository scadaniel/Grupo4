using Microsoft.Maui.Controls;
using App_v3.Views;
using App_v3.Services;
using System.Collections.ObjectModel;

namespace App_v3.Views;

public partial class MenuReportesPage : ContentPage
{
    public MenuReportesPage()
    {
        InitializeComponent();
        BindingContext = new MenuReportesViewModel();
    }

    private void OnBotonMenuFlyoutClicked(object sender, EventArgs e)
    {
        if (Shell.Current != null)
        {
            Shell.Current.FlyoutIsPresented = !Shell.Current.FlyoutIsPresented;
        }
    }

    private async void OnVerReporteContactosClicked(object sender, EventArgs e)
    {
        try
        {
            await Shell.Current.GoToAsync(nameof(ReporteContactosPage));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo abrir el reporte: {ex.Message}", "OK");
        }
    }
}

public class MenuReportesViewModel : BindableObject
{
    public System.Windows.Input.ICommand IrAsistenciasCommand { get; }
    public System.Windows.Input.ICommand IrEntregasTpCommand { get; }
    public System.Windows.Input.ICommand VerReporteRiesgoComisionCommand { get; }

    public ObservableCollection<string> ListaComisiones { get; } = new();

    private string? _comisionSeleccionada;
    public string? ComisionSeleccionada
    {
        get => _comisionSeleccionada;
        set
        {
            _comisionSeleccionada = value;
            OnPropertyChanged();
        }
    }

    public MenuReportesViewModel()
    {
        IrAsistenciasCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(ReporteAsistenciasPage)));
        IrEntregasTpCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(ReporteEntregasTpPage)));

        VerReporteRiesgoComisionCommand = new Command(async () =>
        {
            if (string.IsNullOrWhiteSpace(ComisionSeleccionada))
            {
                await Shell.Current.DisplayAlert("Aviso", "Por favor, seleccione una comisión antes de continuar.", "OK");
                return;
            }

            await Shell.Current.GoToAsync($"{nameof(ReporteRiesgoComisionPage)}?Comision={Uri.EscapeDataString(ComisionSeleccionada)}");
        });

        _ = CargarComisionesAsync();
    }

    private async Task CargarComisionesAsync()
    {
        try
        {
            // Obtenemos el servicio de base de datos desde el contenedor de MAUI
            var databaseService = IPlatformApplication.Current?.Services.GetService<DatabaseService>();
            if (databaseService != null)
            {
                var estudiantes = await databaseService.GetEstudiantesAsync();
                var comisiones = estudiantes
                    .Select(e => e.Comision)
                    .Where(c => !string.IsNullOrEmpty(c))
                    .Distinct()
                    .OrderBy(c => c)
                    .ToList();

                ListaComisiones.Clear();
                foreach (var com in comisiones)
                {
                    ListaComisiones.Add(com);
                }
            }
        }
        catch (Exception)
        {
            // Manejo silencioso o log si falla la carga inicial
        }
    }
}