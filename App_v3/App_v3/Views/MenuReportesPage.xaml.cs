using Microsoft.Maui.Controls;
using App_v3.Views;

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

    // Este método es el que se conecta directamente con Clicked="OnVerReporteContactosClicked" del XAML
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

public class MenuReportesViewModel
{
    public System.Windows.Input.ICommand IrAsistenciasCommand { get; }
    public System.Windows.Input.ICommand IrEntregasTpCommand { get; }

    public MenuReportesViewModel()
    {
        IrAsistenciasCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(ReporteAsistenciasPage)));
        IrEntregasTpCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(ReporteEntregasTpPage)));
    }
}