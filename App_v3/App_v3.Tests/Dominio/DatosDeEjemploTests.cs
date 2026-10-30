using App_v3.Dominio;

namespace App_v3.Tests;

[TestClass]
public sealed class DatosDeEjemploTests
{
    private readonly EscenarioDePrueba _e = new();
    private DatosDeEjemplo Ejemplo => new(_e.Repositorio, _e.Reloj);

    private async Task<Comision> ComisionAsync(string nombre) =>
        (await _e.Comisiones.ListarAsync()).Single(c => c.Nombre == nombre);

    [TestMethod]
    public async Task CargaVariasMateriasComisionesYEstudiantes()
    {
        await Ejemplo.CargarAsync();

        Assert.HasCount(2, await _e.Materias.ListarAsync());
        Assert.HasCount(3, await _e.Comisiones.ListarAsync());
        Assert.HasCount(12, await _e.Estudiantes.ListarAsync());
    }

    [TestMethod]
    public async Task CargarDosVecesNoDuplicaNada()
    {
        await Ejemplo.CargarAsync();
        await Ejemplo.CargarAsync();

        Assert.HasCount(2, await _e.Materias.ListarAsync());
        Assert.HasCount(12, await _e.Estudiantes.ListarAsync());
    }

    [TestMethod]
    public async Task ElPanelDeEjemploTieneLosTresNivelesDeRiesgoOrdenados()
    {
        await Ejemplo.CargarAsync();

        var panel = await _e.Seguimiento.PanelAsync((await ComisionAsync("1K1")).Id);

        Assert.HasCount(8, panel);
        Assert.IsTrue(panel.Any(f => f.Nivel == NivelRiesgo.Alto));
        Assert.IsTrue(panel.Any(f => f.Nivel == NivelRiesgo.Medio));
        Assert.IsTrue(panel.Any(f => f.Nivel == NivelRiesgo.Bajo));
        Assert.AreEqual(NivelRiesgo.Alto, panel[0].Nivel);
        Assert.AreEqual(NivelRiesgo.Bajo, panel[^1].Nivel);
    }

    [TestMethod]
    public async Task ElEstudianteDadoDeBajaNoEstaEnElPanelPeroSiEnLosInactivos()
    {
        await Ejemplo.CargarAsync();
        var comision = await ComisionAsync("1K1");

        var panel = await _e.Seguimiento.PanelAsync(comision.Id);
        var inactivos = await _e.Inscripciones.ListarInactivosAsync(comision.Id);

        Assert.HasCount(1, inactivos);
        Assert.IsFalse(panel.Any(f => f.Estudiante.Id == inactivos[0].Id));
        var ficha = await _e.Seguimiento.ObtenerFichaAsync(inactivos[0].Id, comision.Id);
        Assert.IsNotEmpty(ficha.Asistencias);
    }

    [TestMethod]
    public async Task ElFiltroDeNoContactadosEncuentraAAlgunosPeroNoATodos()
    {
        await Ejemplo.CargarAsync();
        var comision = await ComisionAsync("1K1");

        var todos = await _e.Seguimiento.PanelAsync(comision.Id);
        var sinContacto = await _e.Seguimiento.PanelAsync(comision.Id, soloNoContactadosEstaSemana: true);

        Assert.IsNotEmpty(sinContacto);
        Assert.IsTrue(sinContacto.Count < todos.Count);
        Assert.IsTrue(sinContacto.All(f => f.Nivel != NivelRiesgo.Bajo));
        Assert.IsTrue(todos.Any(f => f.UltimoContacto is not null));
    }

    [TestMethod]
    public async Task UnEstudianteDeEjemploCursaEnDosComisiones()
    {
        await Ejemplo.CargarAsync();

        var enUna = await _e.Inscripciones.ListarActivosAsync((await ComisionAsync("1K1")).Id);
        var enOtra = await _e.Inscripciones.ListarActivosAsync((await ComisionAsync("2K1")).Id);

        Assert.IsTrue(enUna.Select(x => x.Id).Intersect(enOtra.Select(x => x.Id)).Any());
    }

    [TestMethod]
    public async Task LaClaseFuturaDeEjemploNoTieneAsistenciaYSePuedeTomar()
    {
        await Ejemplo.CargarAsync();
        var comision = await ComisionAsync("1K1");
        var futura = (await _e.Calendario.ListarClasesAsync(comision.Id)).Last();

        var planilla = await _e.Asistencias.ObtenerPlanillaAsync(futura.Id);

        Assert.IsTrue(futura.Fecha > _e.Hoy);
        Assert.IsFalse(planilla.YaRegistrada);
        Assert.HasCount(8, planilla.Lineas);
    }

    [TestMethod]
    public async Task TodosLosEstudiantesDeEjemploCumplenLasValidacionesDelAlta()
    {
        await Ejemplo.CargarAsync();

        foreach (var estudiante in await _e.Estudiantes.ListarAsync())
        {
            var datos = new DatosEstudiante(estudiante.Legajo, estudiante.Dni, estudiante.Nombre, estudiante.Apellido, estudiante.Email, estudiante.Telefono);
            ValidacionDeAlta(datos);
        }
    }

    private void ValidacionDeAlta(DatosEstudiante datos)
    {
        // Si el alta pasara la validación pero el estudiante ya existe, se rechaza por duplicado: eso prueba que el formato era válido.
        var ex = Assert.Throws<AggregateException>(() => _e.Estudiantes.AltaAsync(datos).Wait());
        Assert.IsTrue(ex.InnerException is ReglaDeNegocioException { Message: var m } && m.Contains("Ya existe"));
    }
}
