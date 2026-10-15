using App_v3.ViewModels;

namespace App_v3.Views
{
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
            await _viewModel.CargarEstudiantesAsync();
        }
    }
}