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
        bool salir = await DisplayAlertAsync(
            "Cerrar sesión",
            "¿Seguro que desea salir del portal de estudiantes?",
            "Salir",
            "Cancelar");

        if (salir)
        {
            Application.Current!.Windows[0].Page = new BienvenidaPage();
        }
    }

}