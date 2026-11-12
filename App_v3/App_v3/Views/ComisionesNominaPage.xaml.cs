using App_v3.ViewModels;

namespace App_v3.Views;

public partial class ComisionesNominaPage : ContentPage
{
    private readonly ComisionesNominaViewModel _viewModel;

    public ComisionesNominaPage(ComisionesNominaViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_viewModel != null)
        {
            await _viewModel.RecargarDatosAsync();
        }
    }
}