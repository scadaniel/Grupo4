using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using App_v3.Models;
using App_v3.Services;

namespace App_v3.ViewModels
{
    public partial class ConfiguracionViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        public ObservableCollection<Comision> ListaComisiones { get; } = new();

        [ObservableProperty]
        private Comision? _comisionSeleccionada;

        private int _inasistenciasBajo;
        public int InasistenciasBajo
        {
            get => _inasistenciasBajo;
            set { _inasistenciasBajo = value; OnPropertyChanged(); }
        }

        private int _inasistenciasMedio;
        public int InasistenciasMedio
        {
            get => _inasistenciasMedio;
            set { _inasistenciasMedio = value; OnPropertyChanged(); }
        }

        private int _tpsBajo;
        public int TpsBajo
        {
            get => _tpsBajo;
            set { _tpsBajo = value; OnPropertyChanged(); }
        }

        private int _tpsMedio;
        public int TpsMedio
        {
            get => _tpsMedio;
            set { _tpsMedio = value; OnPropertyChanged(); }
        }

        public ICommand GuardarCommand { get; }

        public ConfiguracionViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            GuardarCommand = new Command(async () => await GuardarAsync());
            _ = CargarComisionesAsync();
        }

        private async Task CargarComisionesAsync()
        {
            try
            {
                ListaComisiones.Clear();
                var comisiones = await _databaseService.GetComisionesAsync();
                if (comisiones != null)
                {
                    foreach (var c in comisiones)
                    {
                        ListaComisiones.Add(c);
                    }
                }
            }
            catch (Exception ex)
            {
                if (Shell.Current is not null)
                    await Shell.Current.DisplayAlert("Error", $"No se pudieron cargar las comisiones: {ex.Message}", "OK");
            }
        }

        // Se ejecuta automáticamente al cambiar de comisión en el Picker
        partial void OnComisionSeleccionadaChanged(Comision? value)
        {
            if (value != null)
            {
                InasistenciasBajo = value.InasistenciasBajo;
                InasistenciasMedio = value.InasistenciasMedio;
                TpsBajo = value.TpsBajo;
                TpsMedio = value.TpsMedio;
            }
        }

        private async Task GuardarAsync()
        {
            if (ComisionSeleccionada == null)
            {
                if (Shell.Current is not null)
                    await Shell.Current.DisplayAlert("Atención", "Debe seleccionar una comisión antes de guardar los umbrales.", "OK");
                return;
            }

            // Validaciones lógicas básicas de umbrales
            if (InasistenciasBajo >= InasistenciasMedio)
            {
                if (Shell.Current is not null)
                    await Shell.Current.DisplayAlert("Error de validación", "El límite de inasistencias bajas debe ser menor que el medio.", "OK");
                return;
            }

            try
            {
                // Actualizamos los valores en el objeto de la comisión seleccionada
                ComisionSeleccionada.InasistenciasBajo = InasistenciasBajo;
                ComisionSeleccionada.InasistenciasMedio = InasistenciasMedio;
                ComisionSeleccionada.TpsBajo = TpsBajo;
                ComisionSeleccionada.TpsMedio = TpsMedio;

                // Impactamos los cambios en la base de datos
                int resultado = await _databaseService.GuardarUmbralesComisionAsync(ComisionSeleccionada);

                if (resultado > 0)
                {
                    if (Shell.Current is not null)
                        await Shell.Current.DisplayAlert("Éxito", $"Se guardaron correctamente los umbrales para la comisión {ComisionSeleccionada.Nombre}.", "OK");
                }
                else
                {
                    if (Shell.Current is not null)
                        await Shell.Current.DisplayAlert("Error", "No se pudo actualizar la base de datos.", "OK");
                }
            }
            catch (Exception ex)
            {
                if (Shell.Current is not null)
                    await Shell.Current.DisplayAlert("Error", $"Ocurrió un error al guardar: {ex.Message}", "OK");
            }
        }
    }
}