using App_v3.Dominio;

namespace App_v3.Tests;

/// <summary>Arma los servicios sobre un repositorio en memoria y crea datos de ejemplo.</summary>
public sealed class EscenarioDePrueba
{
    private int _contador;

    public RepositorioEnMemoria Repositorio { get; } = new();
    public RelojFijo Reloj { get; } = new(new DateTime(2026, 10, 29, 12, 0, 0));
    public EstudianteService Estudiantes { get; }
    public InscripcionService Inscripciones { get; }
    public MateriaService Materias { get; }
    public ComisionService Comisiones { get; }
    public UmbralesService Umbrales { get; }
    public CalendarioService Calendario { get; }
    public AsistenciaService Asistencias { get; }
    public EntregaService Entregas { get; }
    public ContactoService Contactos { get; }
    public SeguimientoService Seguimiento { get; }
    public ReporteService Reportes { get; }

    public DateTime Hoy => Reloj.Hoy;

    public EscenarioDePrueba()
    {
        Estudiantes = new EstudianteService(Repositorio);
        Inscripciones = new InscripcionService(Repositorio);
        Materias = new MateriaService(Repositorio);
        Comisiones = new ComisionService(Repositorio);
        Umbrales = new UmbralesService(Repositorio);
        Calendario = new CalendarioService(Repositorio);
        Asistencias = new AsistenciaService(Repositorio, Inscripciones);
        Entregas = new EntregaService(Repositorio, Inscripciones);
        Contactos = new ContactoService(Repositorio, Inscripciones);
        Seguimiento = new SeguimientoService(Repositorio, Inscripciones, Umbrales, Reloj);
        Reportes = new ReporteService(Repositorio, Inscripciones, Reloj);
    }

    public Task<Estudiante> NuevoEstudianteAsync(
        string legajo = "L1", string dni = "30111222", string nombre = "Juan", string apellido = "Pérez") =>
        Estudiantes.AltaAsync(new DatosEstudiante(legajo, dni, nombre, apellido, $"{legajo}@mail.com"));

    public Task<Materia> NuevaMateriaAsync(string nombre = "Programación") => Materias.AltaAsync(nombre);

    public async Task<Comision> NuevaComisionAsync(string nombre = "Comisión A")
    {
        var materia = await NuevaMateriaAsync($"Materia de {nombre}");
        return await Comisiones.AltaAsync(materia.Id, nombre);
    }

    /// <summary>Crea un estudiante con legajo y DNI únicos y lo inscribe en la comisión.</summary>
    public async Task<Estudiante> NuevoInscriptoAsync(Comision comision, string nombre = "Ana", string apellido = "Gómez")
    {
        _contador++;
        var estudiante = await NuevoEstudianteAsync($"L{_contador}", (30000000 + _contador).ToString(), nombre, apellido);
        await Inscripciones.InscribirAsync(estudiante.Id, comision.Id);
        return estudiante;
    }

    public Task<ClaseDictada> NuevaClaseAsync(Comision comision, int diasDesdeHoy, string tema = "Tema") =>
        Calendario.AgregarClaseAsync(comision.Id, Hoy.AddDays(diasDesdeHoy), tema);

    public Task<TrabajoPractico> NuevoTrabajoAsync(Comision comision, string nombre, int diasDesdeHoy) =>
        Calendario.AgregarTrabajoAsync(comision.Id, nombre, Hoy.AddDays(diasDesdeHoy));

    /// <summary>Toma la asistencia de una clase marcando como ausentes a los estudiantes indicados.</summary>
    public async Task TomarAsistenciaAsync(ClaseDictada clase, params Estudiante[] ausentes)
    {
        var planilla = await Asistencias.ObtenerPlanillaAsync(clase.Id);
        var lineas = planilla.Lineas.Select(l => l with { Presente = !ausentes.Any(a => a.Id == l.Estudiante.Id) });
        await Asistencias.GuardarAsync(clase.Id, lineas);
    }
}
