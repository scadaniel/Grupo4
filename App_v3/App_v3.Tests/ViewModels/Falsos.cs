using App_v3.Dominio;
using App_v3.ViewModels;

namespace App_v3.Tests;

/// <summary>Diálogo de las pruebas: guarda los mensajes que la pantalla habría mostrado y responde las preguntas con lo que se le indique.</summary>
public sealed class DialogoFalso : IDialogService
{
    public List<(string Titulo, string Mensaje)> Alertas { get; } = [];
    public List<string> Preguntas { get; } = [];
    public bool RespuestaAConfirmar { get; set; } = true;

    public (string Titulo, string Mensaje) Ultima => Alertas[^1];

    public Task AlertAsync(string titulo, string mensaje)
    {
        Alertas.Add((titulo, mensaje));
        return Task.CompletedTask;
    }

    public Task<bool> ConfirmarAsync(string titulo, string mensaje)
    {
        Preguntas.Add(titulo);
        return Task.FromResult(RespuestaAConfirmar);
    }
}

public sealed class NavegacionFalsa : INavegacionService
{
    public List<string> Rutas { get; } = [];
    public int Vueltas { get; private set; }

    public Task IrAAsync(string ruta)
    {
        Rutas.Add(ruta);
        return Task.CompletedTask;
    }

    public Task VolverAsync()
    {
        Vueltas++;
        return Task.CompletedTask;
    }
}

public sealed class EnlaceFalso : IEnlaceService
{
    public List<string> Abiertos { get; } = [];
    public bool PuedeAbrir { get; set; } = true;

    public Task<bool> AbrirAsync(string direccion)
    {
        Abiertos.Add(direccion);
        return Task.FromResult(PuedeAbrir);
    }
}

/// <summary>Arma los ViewModels de las pantallas sobre el mismo escenario de dominio que usan las demás pruebas.</summary>
public sealed class EscenarioDeVistas
{
    public EscenarioDePrueba Dominio { get; } = new();
    public DialogoFalso Dialogos { get; } = new();
    public NavegacionFalsa Navegacion { get; } = new();
    public EnlaceFalso Enlaces { get; } = new();
    public SelectorComisionViewModel Selector { get; }

    public EscenarioDeVistas()
    {
        Selector = new SelectorComisionViewModel(Dominio.Materias, Dominio.Comisiones, Dialogos);
    }

    public MateriasViewModel Materias() => new(Selector, Dominio.Materias, Dominio.Comisiones, Dialogos);

    public EstudiantesViewModel Estudiantes() =>
        new(Selector, Dominio.Estudiantes, Dominio.Inscripciones, Dominio.Seguimiento, Dominio.Reloj, FormularioContacto(), Dialogos);

    public CalendarioViewModel Calendario() => new(Selector, Dominio.Calendario, Dominio.Reloj, Dialogos);

    public AsistenciaViewModel Asistencia() => new(Selector, Dominio.Asistencias, Dominio.Calendario, Dialogos);

    public EntregasViewModel Entregas() => new(Selector, Dominio.Entregas, Dominio.Calendario, Dialogos);

    public FormularioContactoViewModel FormularioContacto() =>
        new(Selector, Dominio.Contactos, Enlaces, Dominio.Reloj, Dialogos);

    public SeguimientoViewModel Seguimiento() => new(Selector, Dominio.Seguimiento, FormularioContacto(), Dialogos);

    public UmbralesViewModel Umbrales() => new(Dominio.Umbrales, Dialogos);

    public ReporteAsistenciasViewModel ReporteAsistencias() =>
        new(Selector, Dominio.Reportes, Navegacion, Dominio.Reloj, Dialogos);

    public ReporteEntregasViewModel ReporteEntregas() =>
        new(Selector, Dominio.Reportes, Dominio.Calendario, Navegacion, Dialogos);

    /// <summary>Crea una comisión con estudiantes inscriptos y deja la pantalla apuntando a ella.</summary>
    public async Task<Comision> ConComisionAsync(params (string Nombre, string Apellido)[] estudiantes)
    {
        var comision = await Dominio.NuevaComisionAsync("1K1");
        foreach (var (nombre, apellido) in estudiantes)
            await Dominio.NuevoInscriptoAsync(comision, nombre, apellido);

        await Selector.CargarAsync();
        return comision;
    }
}
