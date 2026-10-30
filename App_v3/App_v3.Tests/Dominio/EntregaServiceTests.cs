using App_v3.Dominio;

namespace App_v3.Tests;

[TestClass]
public sealed class EntregaServiceTests
{
    private readonly EscenarioDePrueba _e = new();

    [TestMethod]
    public async Task CA5_3_LaPlanillaListaATodosLosEstudiantesActivosDeLaComisionSinEntregar()
    {
        var comision = await _e.NuevaComisionAsync("Comisión A");
        var otra = await _e.NuevaComisionAsync("Comisión B");
        await _e.NuevoInscriptoAsync(comision, "Ana", "Gómez");
        var baja = await _e.NuevoInscriptoAsync(comision, "Beto", "Pérez");
        await _e.NuevoInscriptoAsync(otra, "Carla", "Díaz");
        await _e.Inscripciones.DarDeBajaAsync(baja.Id, comision.Id, _e.Hoy);
        var trabajo = await _e.NuevoTrabajoAsync(comision, "TP 1", 7);

        var planilla = await _e.Entregas.ObtenerPlanillaAsync(trabajo.Id);

        Assert.HasCount(1, planilla.Lineas);
        Assert.IsFalse(planilla.Lineas[0].Entregado);
        Assert.IsFalse(planilla.YaRegistrada);
    }

    [TestMethod]
    public async Task CA5_1_YCA5_2_SeRegistraQuienEntregoYQuienNoEntrego()
    {
        var comision = await _e.NuevaComisionAsync();
        var ana = await _e.NuevoInscriptoAsync(comision, "Ana", "Gómez");
        var beto = await _e.NuevoInscriptoAsync(comision, "Beto", "Pérez");
        var trabajo = await _e.NuevoTrabajoAsync(comision, "TP 1", 7);

        await _e.Entregas.GuardarAsync(trabajo.Id, [new LineaEntrega(ana, true), new LineaEntrega(beto, false)]);

        var guardadas = await _e.Repositorio.ListarAsync<Entrega>();
        Assert.HasCount(2, guardadas);
        Assert.IsTrue(guardadas.Single(g => g.EstudianteId == ana.Id).Entregado);
        Assert.IsFalse(guardadas.Single(g => g.EstudianteId == beto.Id).Entregado);
        Assert.IsTrue(guardadas.All(g => g.TrabajoPracticoId == trabajo.Id));
    }

    [TestMethod]
    public async Task CA5_4_AlVolverASeleccionarElTrabajoSeMuestraLoGuardado()
    {
        var comision = await _e.NuevaComisionAsync();
        var ana = await _e.NuevoInscriptoAsync(comision, "Ana", "Gómez");
        await _e.NuevoInscriptoAsync(comision, "Beto", "Pérez");
        var trabajo = await _e.NuevoTrabajoAsync(comision, "TP 1", 7);
        await _e.Entregas.GuardarAsync(trabajo.Id, [new LineaEntrega(ana, true)]);

        var planilla = await _e.Entregas.ObtenerPlanillaAsync(trabajo.Id);

        Assert.IsTrue(planilla.YaRegistrada);
        Assert.IsTrue(planilla.Lineas.Single(l => l.Estudiante.Id == ana.Id).Entregado);
        Assert.IsFalse(planilla.Lineas.Single(l => l.Estudiante.Nombre == "Beto").Entregado);
    }

    [TestMethod]
    public async Task GuardarDeNuevoCorrigeLaEntregaSinDuplicarla()
    {
        var comision = await _e.NuevaComisionAsync();
        var ana = await _e.NuevoInscriptoAsync(comision);
        var trabajo = await _e.NuevoTrabajoAsync(comision, "TP 1", 7);
        await _e.Entregas.GuardarAsync(trabajo.Id, [new LineaEntrega(ana, false)]);

        await _e.Entregas.GuardarAsync(trabajo.Id, [new LineaEntrega(ana, true)]);

        var guardadas = await _e.Repositorio.ListarAsync<Entrega>();
        Assert.HasCount(1, guardadas);
        Assert.IsTrue(guardadas[0].Entregado);
    }

    [TestMethod]
    public async Task CA6_3_RegistrarUnaEntregaActualizaElRiesgo()
    {
        var comision = await _e.NuevaComisionAsync();
        var ana = await _e.NuevoInscriptoAsync(comision);
        var trabajo = await _e.NuevoTrabajoAsync(comision, "TP 1", -2);
        Assert.AreEqual(NivelRiesgo.Medio, (await _e.Seguimiento.PanelAsync(comision.Id))[0].Nivel);

        await _e.Entregas.GuardarAsync(trabajo.Id, [new LineaEntrega(ana, true)]);

        Assert.AreEqual(NivelRiesgo.Bajo, (await _e.Seguimiento.PanelAsync(comision.Id))[0].Nivel);
    }

    [TestMethod]
    public async Task NoSeGuardaLaEntregaDeUnEstudianteQueNoEstaActivoEnLaComision()
    {
        var comision = await _e.NuevaComisionAsync("Comisión A");
        var otra = await _e.NuevaComisionAsync("Comisión B");
        await _e.NuevoInscriptoAsync(comision);
        var ajeno = await _e.NuevoInscriptoAsync(otra, "Carla", "Díaz");
        var trabajo = await _e.NuevoTrabajoAsync(comision, "TP 1", 7);

        await Assert.ThrowsAsync<ReglaDeNegocioException>(
            () => _e.Entregas.GuardarAsync(trabajo.Id, [new LineaEntrega(ajeno, true)]));

        Assert.IsEmpty(await _e.Repositorio.ListarAsync<Entrega>());
    }

    [TestMethod]
    public async Task SinEstudiantesActivosNoSePuedeGuardar()
    {
        var comision = await _e.NuevaComisionAsync();
        var trabajo = await _e.NuevoTrabajoAsync(comision, "TP 1", 7);

        var ex = await Assert.ThrowsAsync<ReglaDeNegocioException>(() => _e.Entregas.GuardarAsync(trabajo.Id, []));

        Assert.IsTrue(ex.Message.Contains("No hay estudiantes activos"));
    }

    [TestMethod]
    public async Task UnTrabajoQueNoExisteSeRechaza()
    {
        var ex = await Assert.ThrowsAsync<ReglaDeNegocioException>(() => _e.Entregas.ObtenerPlanillaAsync(999));

        Assert.IsTrue(ex.Message.Contains("trabajo práctico"));
    }
}
