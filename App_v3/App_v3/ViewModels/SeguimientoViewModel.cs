using System.Collections.ObjectModel;
using System.Windows.Input;
using App_v3.Dominio;

namespace App_v3.ViewModels;

/// <summary>H8, H10 y H11: panel de seguimiento ordenado por riesgo, con el registro de contactos.</summary>
public class SeguimientoViewModel : PaginaViewModel
{
    private readonly SeguimientoService _seguimiento;
    private bool _soloNoContactados;

    public SeguimientoViewModel(
        SelectorComisionViewModel selector,
        SeguimientoService seguimiento,
        FormularioContactoViewModel contacto,
        IDialogService dialogos) : base(selector, dialogos)
    {
        _seguimiento = seguimiento;
        Contacto = contacto;
        Contacto.AlRegistrar = CargarAsync;
        AlternarFiltroCommand = Comando(AlternarFiltroAsync);
    }

    public ObservableCollection<FilaSeguimientoViewModel> Filas { get; } = [];
    public FormularioContactoViewModel Contacto { get; }
    public ICommand AlternarFiltroCommand { get; }

    public bool SoloNoContactados
    {
        get => _soloNoContactados;
        private set
        {
            if (SetProperty(ref _soloNoContactados, value)) OnPropertyChanged(nameof(TextoFiltro));
        }
    }

    public string TextoFiltro => SoloNoContactados ? "Ver todos" : "No contactados esta semana";
    public bool HayFilas => Filas.Count > 0;

    // CA 8.1 y CA 11.2: de mayor a menor riesgo; el filtro deja a los que no fueron contactados esta semana.
    public override async Task CargarAsync()
    {
        Filas.Clear();
        Contacto.Cerrar();
        if (Selector.ComisionId is { } comisionId)
            foreach (var situacion in await _seguimiento.PanelAsync(comisionId, SoloNoContactados))
                Filas.Add(new FilaSeguimientoViewModel(situacion, fila => { Contacto.Abrir(fila.Estudiante); return Task.CompletedTask; }));

        OnPropertyChanged(nameof(HayFilas));
    }

    private async Task AlternarFiltroAsync()
    {
        SoloNoContactados = !SoloNoContactados;
        await CargarAsync();
    }
}
