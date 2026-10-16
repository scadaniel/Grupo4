using App_v3.ViewModels;

namespace App_v3.Views;

public partial class TomaAsistenciaPage : ContentPage
{
    public TomaAsistenciaPage(TomaAsistenciaViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}