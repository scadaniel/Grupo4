namespace App_v3.Views;

public partial class WelcomePage : ContentPage
{
    public WelcomePage()
    {
        InitializeComponent();
    }

    private async void OnIngresarClicked(object sender, EventArgs e)
    {
        // Esto redirige al Shell principal o ruta raíz de tu app
        await Shell.Current.GoToAsync("//MainPage");
        // O si prefieres usar la navegación modal / routing, puedes ajustarlo según tu estructura.
    }
}