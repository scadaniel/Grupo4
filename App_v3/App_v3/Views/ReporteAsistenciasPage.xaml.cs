using App_v3.ViewModels;
using App_v3.Services;

namespace App_v3.Views;

public partial class ReporteAsistenciasPage : ContentPage
{
    private readonly ReporteAsistenciasViewModel _viewModel;

    public ReporteAsistenciasPage(DatabaseService databaseService)
    {
        InitializeComponent();

        _viewModel = new ReporteAsistenciasViewModel(databaseService);
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