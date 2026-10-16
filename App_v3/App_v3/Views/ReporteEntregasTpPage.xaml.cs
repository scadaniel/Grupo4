using App_v3.ViewModels;
using App_v3.Services;

namespace App_v3.Views;

public partial class ReporteEntregasTpPage : ContentPage
{
    private readonly ReporteEntregasTpViewModel _viewModel;

    public ReporteEntregasTpPage(DatabaseService databaseService)
    {
        InitializeComponent();
        _viewModel = new ReporteEntregasTpViewModel(databaseService);
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_viewModel != null)
        {
            await _viewModel.CargarReporteAsync();
        }
    }
}