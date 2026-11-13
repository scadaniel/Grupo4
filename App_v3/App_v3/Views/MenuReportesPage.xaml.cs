using Microsoft.Maui.Controls;
using App_v3.Views;
using App_v3.Services;
using System.Collections.ObjectModel;

namespace App_v3.Views
{
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
                var databaseService = IPlatformApplication.Current?.Services.GetService<DatabaseService>();
                if (databaseService != null)
                {
                    // 🌟 Usamos la función exclusiva para obtener comisiones limpias, sin duplicados y actualizadas
                    var comisionesDb = await databaseService.GetComisionesParaReportesExclusivoAsync();

                    var nombresComisiones = comisionesDb
                        .Where(c => !string.IsNullOrWhiteSpace(c.Nombre))
                        .Select(c => c.Nombre.Trim())
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .OrderBy(c => c)
                        .ToList();

                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        ListaComisiones.Clear();
                        foreach (var com in nombresComisiones)
                        {
                            ListaComisiones.Add(com);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Error Comisiones Reporte]: {ex.Message}");
            }
        }
    }
}