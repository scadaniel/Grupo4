namespace App_v3.Views;

/// <summary>Aviso de "no hay comisión" con un botón que lleva a la pestaña Materias. Se enlaza al selector global.</summary>
public partial class EstadoVacioView : ContentView
{
    public EstadoVacioView()
    {
        InitializeComponent();
    }

    private async void AlIrAMaterias(object? sender, EventArgs e) => await Shell.Current.GoToAsync("//Materias");
}