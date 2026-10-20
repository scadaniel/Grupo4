using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using App_v3.Models;
using App_v3.Services;

namespace App_v3.ViewModels;

public class ConfiguracionRiesgoViewModel : INotifyPropertyChanged
{
    private readonly IDatabaseService _db;
    private readonly IDialogService _dialog;
    private int _id;

    private int _inasistenciasMedio;
    public int InasistenciasMedio
    {
        get => _inasistenciasMedio;
        set => SetProperty(ref _inasistenciasMedio, value);
    }

    private int _inasistenciasAlto;
    public int InasistenciasAlto
    {
        get => _inasistenciasAlto;
        set => SetProperty(ref _inasistenciasAlto, value);
    }

    private int _tpsNoEntregadosMedio;
    public int TpsNoEntregadosMedio
    {
        get => _tpsNoEntregadosMedio;
        set => SetProperty(ref _tpsNoEntregadosMedio, value);
    }

    private int _tpsNoEntregadosAlto;
    public int TpsNoEntregadosAlto
    {
        get => _tpsNoEntregadosAlto;
        set => SetProperty(ref _tpsNoEntregadosAlto, value);
    }

    public ICommand GuardarCommand { get; }

    public ConfiguracionRiesgoViewModel(IDatabaseService db, IDialogService dialog)
    {
        _db = db;
        _dialog = dialog;

        GuardarCommand = new Command(async () => await GuardarAsync());

        _ = CargarAsync();
    }

    private async Task CargarAsync()
    {
        var lista = await _db.GetAllAsync<ConfiguracionRiesgo>();
        var config = lista.FirstOrDefault() ?? new ConfiguracionRiesgo();

        _id = config.Id;
        InasistenciasMedio = config.InasistenciasMedio;
        InasistenciasAlto = config.InasistenciasAlto;
        TpsNoEntregadosMedio = config.TpsNoEntregadosMedio;
        TpsNoEntregadosAlto = config.TpsNoEntregadosAlto;
    }

    private async Task GuardarAsync()
    {
        var config = new ConfiguracionRiesgo
        {
            Id = _id,
            InasistenciasMedio = InasistenciasMedio,
            InasistenciasAlto = InasistenciasAlto,
            TpsNoEntregadosMedio = TpsNoEntregadosMedio,
            TpsNoEntregadosAlto = TpsNoEntregadosAlto
        };

        // CA 7.3: no se guardan valores contradictorios
        if (!config.EsValida(out var mensaje))
        {
            await _dialog.AlertAsync("Umbrales inválidos", mensaje);
            return;
        }

        // CA 7.1: se registra la nueva configuración
        await _db.SaveAsync(config);
        _id = config.Id;

        await _dialog.AlertAsync("Umbrales guardados", "La configuración de riesgo se actualizó correctamente.");
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void SetProperty<T>(ref T campo, T valor, [CallerMemberName] string? nombre = null)
    {
        if (EqualityComparer<T>.Default.Equals(campo, valor)) return;
        campo = valor;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
    }
}
