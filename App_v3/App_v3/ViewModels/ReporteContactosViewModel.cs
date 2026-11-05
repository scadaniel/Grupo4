using System.Collections.ObjectModel;
using System.Windows.Input;
using App_v3.Models;
using App_v3.Services;

namespace App_v3.ViewModels
{
    public class ReporteContactosViewModel : BindableObject
    {
        private readonly DatabaseService _databaseService;

        public ObservableCollection<Comision> ListaComisiones { get; set; } = new();
        public ObservableCollection<ReporteContactoItemModel> ContactosRealizados { get; set; } = new();

        private Comision? _comisionSeleccionada;
        public Comision? ComisionSeleccionada
        {
            get => _comisionSeleccionada;
            set
            {
                if (_comisionSeleccionada != value)
                {
                    _comisionSeleccionada = value;
                    OnPropertyChanged();
                    _ = CargarReporteContactosAsync();
                }
            }
        }

        public ICommand VolverAlMenuCommand { get; }
        public ICommand CargarComisionesCommand { get; }

        public ReporteContactosViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;

            // Comando para volver al menú anterior
            VolverAlMenuCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
            CargarComisionesCommand = new Command(async () => await CargarComisionesAsync());

            _ = InicializarAsync();
        }

        private async Task InicializarAsync()
        {
            await CargarComisionesAsync();
            if (ListaComisiones.Count > 0 && ComisionSeleccionada == null)
            {
                ComisionSeleccionada = ListaComisiones[0];
            }
        }

        public async Task CargarComisionesAsync()
        {
            try
            {
                var comisiones = await _databaseService.GetComisionesAsync();

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    ListaComisiones.Clear();
                    ListaComisiones.Add(new Comision { Id = 0, Nombre = "Todas las comisiones" });

                    if (comisiones != null)
                    {
                        foreach (var c in comisiones)
                        {
                            ListaComisiones.Add(c);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Error Comisiones Reporte]: {ex.Message}");
            }
        }

        private async Task CargarReporteContactosAsync()
        {
            try
            {
                int? comisionId = (ComisionSeleccionada != null && ComisionSeleccionada.Id > 0) ? ComisionSeleccionada.Id : (int?)null;
                string? comisionNombre = (ComisionSeleccionada != null && ComisionSeleccionada.Id > 0) ? ComisionSeleccionada.Nombre : null;

                var resultados = await _databaseService.GetReporteContactosDetalladoAsync(comisionId ?? 0);

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    ContactosRealizados.Clear();
                    if (resultados != null)
                    {
                        foreach (var item in resultados)
                        {
                            if (comisionNombre == null || item.ComisionNombre?.Trim().Equals(comisionNombre.Trim(), StringComparison.OrdinalIgnoreCase) == true)
                            {
                                ContactosRealizados.Add(item);
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Error Reporte Contactos]: {ex.Message}");
            }
        }
    }
}