using App_v3.ViewModels;

namespace App_v3.Views;

public partial class EstudianteListPage : ContentPage
{
    private readonly EstudianteListViewModel _viewModel;

    public EstudianteListPage(EstudianteListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Recargar la lista y refrescar el estado de los contactos guardados
        await _viewModel.FiltrarYCalcularRiesgoPorComisionAsync();
    }
}