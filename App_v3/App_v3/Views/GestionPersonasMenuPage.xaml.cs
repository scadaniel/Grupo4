namespace App_v3.Views;

public partial class GestionPersonasMenuPage : ContentPage
{
    public GestionPersonasMenuPage()
    {
        InitializeComponent();
    }

    private void OnBotonMenuFlyoutClicked(object sender, EventArgs e)
    {
        if (Shell.Current != null)
        {
            Shell.Current.FlyoutIsPresented = !Shell.Current.FlyoutIsPresented;
        }
    }
}