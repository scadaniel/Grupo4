using App_v3.ViewModels;

namespace App_v3.Views;

/// <summary>Barra fija de Shell: materia y comisión elegidas una sola vez, y acceso a Reportes y Configuración.</summary>
public partial class BarraGlobalView : ContentView
{
    public BarraGlobalView()
    {
        InitializeComponent();
    }

    private async void AlIrAReportes(object? sender, EventArgs e) => await IrAAsync(Rutas.Reportes);

    private async void AlIrAConfiguracion(object? sender, EventArgs e) => await IrAAsync(Rutas.Configuracion);

    private static async Task IrAAsync(string ruta)
    {
        try
        {
            var shell = Shell.Current;
            if (shell.CurrentState.Location.ToString().Contains("/" + ruta)) return;
            if (shell.Navigation.NavigationStack.Count > 1) await shell.Navigation.PopToRootAsync(false);
            await shell.GoToAsync(ruta);
        }
        catch (Exception ex)
        {
            if (Shell.Current is not null) await Shell.Current.DisplayAlertAsync("Error", ex.Message, "Aceptar");
        }
    }

    private async void AlPresionarOpciones(object sender, EventArgs e)
    {
        bool salir = await Shell.Current.DisplayAlert(
            "Cerrar sesión", 
            "¿Seguro que desea salir al menú principal?", 
            "Salir", 
            "Cancelar");

        if (salir)
        {
            Application.Current!.MainPage = new BienvenidaPage();
        }
    }
}