using System.Windows.Input;
using App_v3.Dominio;

namespace App_v3.ViewModels;

/// <summary>
/// H12: reporte de estudiantes en riesgo de la comisión elegida arriba.
/// No calcula nada: muestra lo que dice SeguimientoService, el mismo cálculo que usa el panel (CA2).
/// </summary>
public class ReporteRiesgoViewModel : PaginaViewModel
{
    private readonly SeguimientoService _seguimiento;
    private readonly INavegacionService _navegacion;
    private string _titular = string.Empty;
    private string _detalle = string.Empty;

    public ReporteRiesgoViewModel(
        SelectorComisionViewModel selector, SeguimientoService seguimiento, INavegacionService navegacion, IDialogService dialogos)
        : base(selector, dialogos)
    {
        _seguimiento = seguimiento;
        _navegacion = navegacion;
        VolverCommand = Comando(_navegacion.VolverAsync);
        ActualizarCommand = Comando(CargarAsync);
    }

    public ICommand VolverCommand { get; }
    public ICommand ActualizarCommand { get; }

    /// <summary>La frase principal: "7 de 30 estudiantes en riesgo..." o el aviso de que no hay datos.</summary>
    public string Titular { get => _titular; private set => SetProperty(ref _titular, value); }

    public string Detalle { get => _detalle; private set => SetProperty(ref _detalle, value); }

    public override async Task CargarAsync()
    {
        if (Selector.ComisionId is not { } comisionId)
        {
            Titular = string.Empty;
            Detalle = string.Empty;
            return;
        }

        var resumen = await _seguimiento.ResumenDeRiesgoAsync(comisionId);
        (Titular, Detalle) = Redactar(resumen);
    }

    // CA1 y CA3: el texto depende del estado. Sin datos nunca se escribe "0 en riesgo".
    private static (string Titular, string Detalle) Redactar(ResumenRiesgoComision r) => r.Estado switch
    {
        EstadoResumenRiesgo.SinEstudiantes => (
            "Esta comisión no tiene estudiantes activos.",
            "Inscribí estudiantes para poder calcular el riesgo."),

        EstadoResumenRiesgo.SinDatos => (
            "Todavía no hay datos para calcular el riesgo.",
            $"La comisión tiene {r.TotalActivos} {Estudiantes(r.TotalActivos)}, pero no hay ninguna asistencia ni entrega registrada."),

        _ => (
            $"{r.EnRiesgo} de {r.TotalActivos} {Estudiantes(r.TotalActivos)} en riesgo de abandonar la materia",
            $"Riesgo alto: {r.Alto} · Riesgo medio: {r.Medio}. Se cuenta como en riesgo el nivel Medio o Alto, igual que en el panel de seguimiento.")
    };

    private static string Estudiantes(int cantidad) => cantidad == 1 ? "estudiante" : "estudiantes";
}
