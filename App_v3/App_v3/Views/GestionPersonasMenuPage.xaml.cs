using App_v3.ViewModels;

namespace App_v3.Views;

public partial class GestionPersonasMenuPage : ContentPage
{
    public GestionPersonasMenuPage()
    {
        InitializeComponent();

        // 👈 AQUÍ ASIGNAMOS EL VIEWMODEL PARA QUE FUNCIONEN LOS BOTONES "VER"
        BindingContext = new GestionPersonasMenuViewModel();
    }

    private void OnBotonMenuFlyoutClicked(object sender, EventArgs e)
    {
        if (Shell.Current != null)
        {
            Shell.Current.FlyoutIsPresented = !Shell.Current.FlyoutIsPresented;
        }
    }
}