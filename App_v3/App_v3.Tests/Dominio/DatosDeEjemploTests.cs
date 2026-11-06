using App_v3.Dominio;

namespace App_v3.Tests;

[TestClass]
public sealed class DatosDeEjemploTests
{
    private readonly EscenarioDePrueba _e = new();
    private DatosDeEjemplo Ejemplo => new(_e.Repositorio, _e.Reloj);

    // Busca una comisión que cumpla la condición, sin depender de nombres como "1P1".
    private async Task<Comision> ComisionConAsync(Func<Comision, Task<bool>> condicion)
    {
        foreach (var comision in await _e.Comisiones.ListarAsync())
            if (await condicion(comision)) return comision;

        throw new InvalidOperationException("Los datos de ejemplo no tienen ninguna comisión con esa situación.");
    }

    [TestMethod]
    public async Task CargaMateriasComisionesYEstudiantesRelacionados()
    {
        await Ejemplo.CargarAsync();

        var materias = (await _e.Materias.ListarAsync()).Select(m => m.Id).ToHashSet();
        var comisiones = await _e.Comisiones.ListarAsync();

        Assert.IsNotEmpty(materias);
        Assert.IsNotEmpty(comisiones);
        Assert.IsNotEmpty(await _e.Estudiantes.ListarAsync());
        foreach (var comision in comisiones)
        {
            Assert.IsTrue(materias.Contains(comision.MateriaId));
            Assert.IsNotEmpty(await _e.Inscripciones.ListarActivosAsync(comision.Id));
        }
    }

    [TestMethod]
    public async Task CargarDosVecesNoDuplicaNada()
    {
        await Ejemplo.CargarAsync();
        var materias = (await _e.Materias.ListarAsync()).Count;
        var comisiones = (await _e.Comisiones.ListarAsync()).Count;
        var estudiantes = (await _e.Estudiantes.ListarAsync()).Count;

        await Ejemplo.CargarAsync();

        Assert.HasCount(materias, await _e.Materias.ListarAsync());
        Assert.HasCount(comisiones, await _e.Comisiones.ListarAsync());
        Assert.HasCount(estudiantes, await _e.Estudiantes.ListarAsync());
    }

    [TestMethod]
    public async Task ElPanelDeEjemploTieneLosTresNivelesYSiempreVaDeMayorAMenorRiesgo()
    {
        await Ejemplo.CargarAsync();
        var vistos = new HashSet<NivelRiesgo>();

        foreach (var comision in await _e.Comisiones.ListarAsync())
        {
            var niveles = (await _e.Seguimiento.PanelAsync(comision.Id)).Select(f => f.Nivel).ToList();
            vistos.UnionWith(niveles);
            Assert.IsTrue(niveles.SequenceEqual(niveles.OrderByDescending(n => n)));
        }

        Assert.IsTrue(vistos.Contains(NivelRiesgo.Alto));
        Assert.IsTrue(vistos.Contains(NivelRiesgo.Medio));
        Assert.IsTrue(vistos.Contains(NivelRiesgo.Bajo));
    }

    [TestMethod]
    public async Task ElEstudianteDadoDeBajaNoEstaEnElPanelPeroConservaSuHistorial()
    {
        await Ejemplo.CargarAsync();
        var comision = await ComisionConAsync(async c => (await _e.Inscripciones.ListarInactivosAsync(c.Id)).Count > 0);

        var panel = await _e.Seguimiento.PanelAsync(comision.Id);
        var inactivos = await _e.Inscripciones.ListarInactivosAsync(comision.Id);

        Assert.IsFalse(panel.Any(f => inactivos.Any(i => i.Id == f.Estudiante.Id)));
        var ficha = await _e.Seguimiento.ObtenerFichaAsync(inactivos[0].Id, comision.Id);
        Assert.IsFalse(ficha.Activo);
        Assert.IsNotEmpty(ficha.Asistencias);
    }

    [TestMethod]
    public async Task ElFiltroDeNoContactadosFuncionaYElEjemploTieneCasosParaProbarlo()
    {
        await Ejemplo.CargarAsync();
        var hayContactos = false;
        var hayFiltrados = false;

        foreach (var comision in await _e.Comisiones.ListarAsync())
        {
            var todos = await _e.Seguimiento.PanelAsync(comision.Id);
            var sinContacto = await _e.Seguimiento.PanelAsync(comision.Id, soloNoContactadosEstaSemana: true);

            Assert.IsTrue(sinContacto.Count <= todos.Count);
            Assert.IsTrue(sinContacto.All(f => f.Nivel != NivelRiesgo.Bajo));
            hayContactos |= todos.Any(f => f.UltimoContacto is not null);
            hayFiltrados |= sinContacto.Count > 0;
        }

        Assert.IsTrue(hayContactos);
        Assert.IsTrue(hayFiltrados);
    }

    [TestMethod]
    public async Task LaClaseFuturaDeEjemploNoTieneAsistenciaYSePuedeTomar()
    {
        await Ejemplo.CargarAsync();

        foreach (var comision in await _e.Comisiones.ListarAsync())
        {
            var futura = (await _e.Calendario.ListarClasesAsync(comision.Id)).Last();
            var planilla = await _e.Asistencias.ObtenerPlanillaAsync(futura.Id);
            var activos = await _e.Inscripciones.ListarActivosAsync(comision.Id);

            Assert.IsTrue(futura.Fecha > _e.Hoy);
            Assert.IsFalse(planilla.YaRegistrada);
            Assert.HasCount(activos.Count, planilla.Lineas);
        }
    }

    [TestMethod]
    public async Task TodosLosEstudiantesDeEjemploCumplenLasValidacionesDelAlta()
    {
        await Ejemplo.CargarAsync();

        foreach (var e in await _e.Estudiantes.ListarAsync())
        {
            var datos = new DatosEstudiante(e.Legajo, e.Dni, e.Nombre, e.Apellido, e.Email, e.Telefono);

            // Si el formato fuera inválido fallaría por validación; como ya existe, falla por duplicado.
            var ex = await Assert.ThrowsAsync<ReglaDeNegocioException>(() => _e.Estudiantes.AltaAsync(datos));
            Assert.IsTrue(ex.Message.Contains("Ya existe"));
        }
    }
}