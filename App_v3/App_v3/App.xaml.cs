namespace App_v3;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        
       // Arranca mostrando la página de bienvenida
       MainPage = new Views.BienvenidaPage();
    }

   // protected override Window CreateWindow(IActivationState? activationState) => new(new AppShell());
}
