using App_v3.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace App_v3.ViewModels
{
    public partial class GestionPersonasMenuViewModel : ObservableObject
    {
        [RelayCommand]
        private async Task IrAListadoAsync() => await Shell.Current.GoToAsync(nameof(ListadoEstudiantesPage));

        [RelayCommand]
        private async Task IrAComisionesAsync() => await Shell.Current.GoToAsync(nameof(ComisionesNominaPage));

        [RelayCommand]
        private async Task IrANuevoEstudianteAsync() => await Shell.Current.GoToAsync(nameof(NuevoEstudiantePage));
    }
}