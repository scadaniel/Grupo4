using App_v3.ViewModels;

namespace App_v3.Views;

public partial class EstudiantePortalPage : ContentPage
{
    public EstudiantePortalPage(EstudiantePortalViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void AlPresionarOpciones(object sender, EventArgs e)
    {
        // Cuadro de diálogo nativo con el texto solicitado y los botones de confirmación
        bool salir = await DisplayAlert(
            "Cerrar sesión", 
            "¿Seguro que desea salir del portal de estudiantes?", 
            "Salir", 
            "Cancelar");

        if (salir)
        {
            Application.Current!.MainPage = new BienvenidaPage();
        }
    }
}