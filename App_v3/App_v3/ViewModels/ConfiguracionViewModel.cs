using System.Windows.Input;
using App_v3.Dominio;

namespace App_v3.ViewModels;

/// <summary>H7: umbrales que deciden cuándo el riesgo es Medio o Alto. Se escriben como texto para poder rechazar lo que no es un número.</summary>
public class UmbralesViewModel : BaseViewModel
{
    private readonly UmbralesService _umbrales;
    private string _inasistenciasMedio = string.Empty;
    private string _inasistenciasAlto = string.Empty;
    private string _tpsMedio = string.Empty;
    private string _tpsAlto = string.Empty;

    public UmbralesViewModel(UmbralesService umbrales, IDialogService dialogos) : base(dialogos)
    {
        _umbrales = umbrales;
        GuardarCommand = Comando(GuardarAsync);
    }

    public ICommand GuardarCommand { get; }

    public string InasistenciasMedio { get => _inasistenciasMedio; set => SetProperty(ref _inasistenciasMedio, value); }
    public string InasistenciasAlto { get => _inasistenciasAlto; set => SetProperty(ref _inasistenciasAlto, value); }
    public string TpsMedio { get => _tpsMedio; set => SetProperty(ref _tpsMedio, value); }
    public string TpsAlto { get => _tpsAlto; set => SetProperty(ref _tpsAlto, value); }

    public Task AlMostrarseAsync() => EjecutarAsync(CargarAsync);

    public async Task CargarAsync()
    {
        var actuales = await _umbrales.ObtenerAsync();
        Mostrar(actuales);
    }

    // CA 7.1 y CA 7.3: guarda o explica por qué no. Solo avisa "se guardó" si realmente se guardó.
    private async Task GuardarAsync()
    {
        var guardados = await _umbrales.GuardarAsync(InasistenciasMedio, InasistenciasAlto, TpsMedio, TpsAlto);
        Mostrar(guardados);
        await Dialogos.AlertAsync(
            "Umbrales guardados", "Se registró la nueva configuración. El panel de seguimiento ya muestra los niveles según estos valores.");
    }

    private void Mostrar(UmbralesRiesgo umbrales)
    {
        InasistenciasMedio = umbrales.InasistenciasMedio.ToString();
        InasistenciasAlto = umbrales.InasistenciasAlto.ToString();
        TpsMedio = umbrales.TpsNoEntregadosMedio.ToString();
        TpsAlto = umbrales.TpsNoEntregadosAlto.ToString();
    }
}

/// <summary>Cargar datos de ejemplo y borrar todo lo cargado. Los dos reemplazan los datos, por eso piden confirmación.</summary>
public class DatosViewModel : BaseViewModel
{
    private readonly DatosDeEjemplo _ejemplo;
    private readonly IRepositorio _repositorio;
    private readonly SelectorComisionViewModel _selector;

    public DatosViewModel(
        DatosDeEjemplo ejemplo, IRepositorio repositorio, SelectorComisionViewModel selector, IDialogService dialogos)
        : base(dialogos)
    {
        _ejemplo = ejemplo;
        _repositorio = repositorio;
        _selector = selector;
        CargarEjemploCommand = Comando(CargarEjemploAsync);
        BorrarTodoCommand = Comando(BorrarTodoAsync);
    }

    public ICommand CargarEjemploCommand { get; }
    public ICommand BorrarTodoCommand { get; }

    private async Task CargarEjemploAsync()
    {
        if (!await Dialogos.ConfirmarAsync("Datos de ejemplo", "Se borrará todo lo cargado y se reemplazará por datos de ejemplo. ¿Continuar?"))
            return;

        await _ejemplo.CargarAsync();
        await _selector.CargarAsync();
        await Dialogos.AlertAsync("Datos de ejemplo", "Se cargaron materias, comisiones, estudiantes, clases, asistencias y entregas de ejemplo.");
    }

    private async Task BorrarTodoAsync()
    {
        if (!await Dialogos.ConfirmarAsync("Borrar todo", "Se borrarán TODOS los datos: materias, comisiones, estudiantes y sus registros. No se puede deshacer. ¿Continuar?"))
            return;

        await _repositorio.VaciarAsync();
        await _selector.CargarAsync();
        await Dialogos.AlertAsync("Datos borrados", "La aplicación quedó vacía.");
    }
}

/// <summary>Pantalla de configuración: umbrales de riesgo (H7) y administración de los datos.</summary>
public class ConfiguracionViewModel(UmbralesViewModel umbrales, DatosViewModel datos) : IPantalla
{
    public UmbralesViewModel Umbrales { get; } = umbrales;
    public DatosViewModel Datos { get; } = datos;

    public Task AlMostrarseAsync() => Umbrales.AlMostrarseAsync();
}
