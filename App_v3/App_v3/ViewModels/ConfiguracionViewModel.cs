using System.Windows.Input;
using App_v3.Models;
using App_v3.Services;

namespace App_v3.ViewModels;

public class ConfiguracionViewModel : BindableObject
{
    private readonly ConfiguracionService _configuracionService;

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

    public ConfiguracionViewModel(ConfiguracionService configuracionService)
    {
        _configuracionService = configuracionService;
        CargarValores();

        GuardarCommand = new Command(async () => await GuardarAsync());
    }

    private void CargarValores()
    {
        var umbrales = _configuracionService.ObtenerUmbrales();
        InasistenciasBajo = umbrales.InasistenciasBajoLimite;
        InasistenciasMedio = umbrales.InasistenciasMedioLimite;
        TpsBajo = umbrales.TpsBajoLimite;
        TpsMedio = umbrales.TpsMedioLimite;
    }

    private async Task GuardarAsync()
    {
        var nuevosUmbrales = new UmbralesRiesgo
        {
            InasistenciasBajoLimite = InasistenciasBajo,
            InasistenciasMedioLimite = InasistenciasMedio,
            TpsBajoLimite = TpsBajo,
            TpsMedioLimite = TpsMedio
        };

        var resultado = _configuracionService.GuardarUmbrales(nuevosUmbrales);

        if (!resultado.Exito)
        {
            // CA 7.3: Muestra mensaje de error si hay valores contradictorios
            await Shell.Current.DisplayAlert("Error de validación", resultado.Mensaje, "OK");
        }
        else
        {
            // CA 7.1 y 7.2: Guardado exitoso y aviso de recálculo pendiente
            await Shell.Current.DisplayAlert("Éxito", resultado.Mensaje, "OK");
        }
    }
}