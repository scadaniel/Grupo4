using System.Collections.ObjectModel;
using System.Windows.Input;
using App_v3.Dominio;

namespace App_v3.ViewModels;

/// <summary>Un renglón del calendario: clase o trabajo práctico.</summary>
public record EventoItem(string Tipo, string Descripcion, string Fecha, bool EsClase);

/// <summary>H2: calendario de clases y de trabajos prácticos de la comisión (lista + paneles internos).</summary>
public class CalendarioViewModel : PaginaViewModel
{
    private enum Vista { Lista, NuevaClase, NuevoTrabajo }

    private readonly CalendarioService _calendario;
    private readonly TimeProvider _reloj;
    private DateTime _fechaClase;
    private string _temaClase = string.Empty;
    private DateTime _fechaLimite;
    private string _nombreTrabajo = string.Empty;
    private Vista _vista = Vista.Lista;

    public CalendarioViewModel(
        SelectorComisionViewModel selector, CalendarioService calendario, TimeProvider reloj, IDialogService dialogos)
        : base(selector, dialogos)
    {
        _calendario = calendario;
        _reloj = reloj;
        _fechaClase = Hoy;
        _fechaLimite = Hoy.AddDays(7);
        GuardarClaseCommand = Comando(GuardarClaseAsync);
        GuardarTrabajoCommand = Comando(GuardarTrabajoAsync);
        AbrirNuevaClaseCommand = Comando(() => { IrA(Vista.NuevaClase); return Task.CompletedTask; });
        AbrirNuevoTrabajoCommand = Comando(() => { IrA(Vista.NuevoTrabajo); return Task.CompletedTask; });
        VolverCommand = Comando(() => { IrA(Vista.Lista); return Task.CompletedTask; });
    }

    private DateTime Hoy => _reloj.GetLocalNow().DateTime.Date;

    public ObservableCollection<EventoItem> Eventos { get; } = [];
    public ICommand GuardarClaseCommand { get; }
    public ICommand GuardarTrabajoCommand { get; }
    public ICommand AbrirNuevaClaseCommand { get; }
    public ICommand AbrirNuevoTrabajoCommand { get; }
    public ICommand VolverCommand { get; }

    public bool MostrandoLista => _vista == Vista.Lista;
    public bool MostrandoNuevaClase => _vista == Vista.NuevaClase;
    public bool MostrandoNuevoTrabajo => _vista == Vista.NuevoTrabajo;
    public bool SinEventos => Selector.HayComision && Eventos.Count == 0;

    public DateTime FechaClase { get => _fechaClase; set => SetProperty(ref _fechaClase, value); }
    public string TemaClase { get => _temaClase; set => SetProperty(ref _temaClase, value); }
    public DateTime FechaLimite { get => _fechaLimite; set => SetProperty(ref _fechaLimite, value); }
    public string NombreTrabajo { get => _nombreTrabajo; set => SetProperty(ref _nombreTrabajo, value); }

    // CA 2.3
    public override async Task CargarAsync()
    {
        IrA(Vista.Lista);
        Eventos.Clear();
        if (Selector.ComisionId is not { } comisionId)
        {
            OnPropertyChanged(nameof(SinEventos));
            return;
        }

        foreach (var evento in await _calendario.ListarEventosAsync(comisionId))
            Eventos.Add(new EventoItem(
                evento.Tipo == TipoEvento.Clase ? "Clase" : "Trabajo práctico",
                evento.Descripcion,
                Textos.Fecha(evento.Fecha),
                evento.Tipo == TipoEvento.Clase));

        OnPropertyChanged(nameof(SinEventos));
    }

    // CA 2.1 y CA 2.4
    private async Task GuardarClaseAsync()
    {
        await _calendario.AgregarClaseAsync(ExigirComision(), FechaClase, TemaClase);
        TemaClase = string.Empty;
        await CargarAsync();
    }

    // CA 2.2 y CA 2.4
    private async Task GuardarTrabajoAsync()
    {
        await _calendario.AgregarTrabajoAsync(ExigirComision(), NombreTrabajo, FechaLimite);
        NombreTrabajo = string.Empty;
        await CargarAsync();
    }

    private void IrA(Vista vista)
    {
        _vista = vista;
        OnPropertyChanged(nameof(MostrandoLista));
        OnPropertyChanged(nameof(MostrandoNuevaClase));
        OnPropertyChanged(nameof(MostrandoNuevoTrabajo));
    }
}