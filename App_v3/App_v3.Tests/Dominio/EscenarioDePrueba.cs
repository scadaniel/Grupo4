using App_v3.Dominio;

namespace App_v3.Tests;

/// <summary>Arma los servicios sobre un repositorio en memoria y crea datos de ejemplo.</summary>
public sealed class EscenarioDePrueba
{
    public RepositorioEnMemoria Repositorio { get; } = new();
    public EstudianteService Estudiantes { get; }
    public InscripcionService Inscripciones { get; }
    public MateriaService Materias { get; }
    public ComisionService Comisiones { get; }
    public UmbralesService Umbrales { get; }

    public EscenarioDePrueba()
    {
        Estudiantes = new EstudianteService(Repositorio);
        Inscripciones = new InscripcionService(Repositorio);
        Materias = new MateriaService(Repositorio);
        Comisiones = new ComisionService(Repositorio);
        Umbrales = new UmbralesService(Repositorio);
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
}
