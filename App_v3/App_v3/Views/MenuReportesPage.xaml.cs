using Microsoft.Maui.Controls;

namespace App_v3.Views;

public partial class MenuReportesPage : ContentPage
{
    public MenuReportesPage()
    {
        InitializeComponent();
        BindingContext = new MenuReportesViewModel();
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