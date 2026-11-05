using App_v3.ViewModels;

namespace App_v3.Views; // <-- Debe ser exactamente este namespace

public partial class ReporteContactosPage : ContentPage
{
    public ReporteContactosPage(ReporteContactosViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}