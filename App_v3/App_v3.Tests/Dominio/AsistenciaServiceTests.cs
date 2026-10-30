using App_v3.Dominio;

namespace App_v3.Tests;

[TestClass]
public sealed class AsistenciaServiceTests
{
    private readonly EscenarioDePrueba _e = new();

    [TestMethod]
    public async Task CA3_1_TodosLosActivosQuedanPresentesPorDefecto()
    {
        var comision = await _e.NuevaComisionAsync();
        await _e.NuevoInscriptoAsync(comision, "Ana", "Gómez");
        await _e.NuevoInscriptoAsync(comision, "Beto", "Pérez");
        var clase = await _e.NuevaClaseAsync(comision, -1);

        var planilla = await _e.Asistencias.ObtenerPlanillaAsync(clase.Id);

        Assert.HasCount(2, planilla.Lineas);
        Assert.IsTrue(planilla.Lineas.All(l => l.Presente));
        Assert.IsFalse(planilla.YaRegistrada);
    }

    [TestMethod]
    public async Task CA3_1_LosInactivosYLosDeOtraComisionNoAparecenEnLaGrilla()
    {
        var comision = await _e.NuevaComisionAsync("Comisión A");
        var otra = await _e.NuevaComisionAsync("Comisión B");
        var activo = await _e.NuevoInscriptoAsync(comision, "Ana", "Gómez");
        var inactivo = await _e.NuevoInscriptoAsync(comision, "Beto", "Pérez");
        await _e.NuevoInscriptoAsync(otra, "Carla", "Díaz");
        await _e.Inscripciones.DarDeBajaAsync(inactivo.Id, comision.Id, _e.Hoy);
        var clase = await _e.NuevaClaseAsync(comision, -1);

        var planilla = await _e.Asistencias.ObtenerPlanillaAsync(clase.Id);

        Assert.HasCount(1, planilla.Lineas);
        Assert.AreEqual(activo.Id, planilla.Lineas[0].Estudiante.Id);
    }

    [TestMethod]
    public async Task CA3_1_SinEstudiantesActivosNoSePuedeGuardar()
    {
        var comision = await _e.NuevaComisionAsync();
        var clase = await _e.NuevaClaseAsync(comision, -1);

        var ex = await Assert.ThrowsAsync<ReglaDeNegocioException>(
            () => _e.Asistencias.GuardarAsync(clase.Id, []));

        Assert.IsTrue(ex.Message.Contains("No hay estudiantes activos"));
    }

    [TestMethod]
    public async Task CA3_2_YCA3_3_DesmarcarUnEstudianteLoGuardaComoAusenteYLosDemasComoPresentes()
    {
        var comision = await _e.NuevaComisionAsync();
        var ana = await _e.NuevoInscriptoAsync(comision, "Ana", "Gómez");
        var beto = await _e.NuevoInscriptoAsync(comision, "Beto", "Pérez");
        var clase = await _e.NuevaClaseAsync(comision, -1);

        await _e.TomarAsistenciaAsync(clase, beto);

        var guardadas = await _e.Repositorio.ListarAsync<Asistencia>();
        Assert.HasCount(2, guardadas);
        Assert.AreEqual(EstadoAsistencia.Presente, guardadas.Single(a => a.EstudianteId == ana.Id).Estado);
        Assert.AreEqual(EstadoAsistencia.Ausente, guardadas.Single(a => a.EstudianteId == beto.Id).Estado);
        Assert.IsTrue(guardadas.All(a => a.ClaseDictadaId == clase.Id));
    }

    [TestMethod]
    public async Task CA3_4_SiLaClaseYaTieneRegistroLaPlanillaLoIndicaYTraeLosEstadosGuardados()
    {
        var comision = await _e.NuevaComisionAsync();
        await _e.NuevoInscriptoAsync(comision, "Ana", "Gómez");
        var beto = await _e.NuevoInscriptoAsync(comision, "Beto", "Pérez");
        var clase = await _e.NuevaClaseAsync(comision, -1);
        await _e.TomarAsistenciaAsync(clase, beto);

        var planilla = await _e.Asistencias.ObtenerPlanillaAsync(clase.Id);

        Assert.IsTrue(planilla.YaRegistrada);
        Assert.IsFalse(planilla.Lineas.Single(l => l.Estudiante.Id == beto.Id).Presente);
        Assert.IsTrue(planilla.Lineas.Single(l => l.Estudiante.Nombre == "Ana").Presente);
    }

    [TestMethod]
    public async Task CA3_4_GuardarDosVecesLaMismaClaseSeRechazaYSugiereModificar()
    {
        var comision = await _e.NuevaComisionAsync();
        await _e.NuevoInscriptoAsync(comision);
        var clase = await _e.NuevaClaseAsync(comision, -1);
        await _e.TomarAsistenciaAsync(clase);

        var ex = await Assert.ThrowsAsync<ReglaDeNegocioException>(() => _e.TomarAsistenciaAsync(clase));

        Assert.IsTrue(ex.Message.Contains("Ya existe un registro"));
        Assert.IsTrue(ex.Message.Contains("Modificar asistencia"));
        Assert.HasCount(1, await _e.Repositorio.ListarAsync<Asistencia>());
    }

    [TestMethod]
    public async Task CA3_5_UnaAusenciaJustificadaSeGuardaYNoSumaInasistencias()
    {
        var comision = await _e.NuevaComisionAsync();
        var ana = await _e.NuevoInscriptoAsync(comision);
        var clase = await _e.NuevaClaseAsync(comision, -1);

        await _e.Asistencias.GuardarAsync(clase.Id, [new LineaAsistencia(ana, Presente: false, Justificada: true)]);

        var guardada = (await _e.Repositorio.ListarAsync<Asistencia>()).Single();
        Assert.AreEqual(EstadoAsistencia.AusenteJustificado, guardada.Estado);
        var panel = await _e.Seguimiento.PanelAsync(comision.Id);
        Assert.AreEqual(0, panel[0].Inasistencias);
    }

    [TestMethod]
    public async Task SiElEstudianteEstuvoPresenteLaJustificacionSeIgnora()
    {
        var comision = await _e.NuevaComisionAsync();
        var ana = await _e.NuevoInscriptoAsync(comision);
        var clase = await _e.NuevaClaseAsync(comision, -1);

        await _e.Asistencias.GuardarAsync(clase.Id, [new LineaAsistencia(ana, Presente: true, Justificada: true)]);

        var guardada = (await _e.Repositorio.ListarAsync<Asistencia>()).Single();
        Assert.IsFalse(guardada.Justificada);
        Assert.AreEqual(EstadoAsistencia.Presente, guardada.Estado);
    }

    [TestMethod]
    public async Task CA4_3_CorregirActualizaElRegistroSinDuplicarloYRecalculaElRiesgo()
    {
        var comision = await _e.NuevaComisionAsync();
        var ana = await _e.NuevoInscriptoAsync(comision);
        await _e.Umbrales.GuardarAsync("1", "2", "1", "3");
        var clase = await _e.NuevaClaseAsync(comision, -1);
        await _e.TomarAsistenciaAsync(clase, ana);
        Assert.AreEqual(NivelRiesgo.Medio, (await _e.Seguimiento.PanelAsync(comision.Id))[0].Nivel);

        await _e.Asistencias.CorregirAsync(clase.Id, [new LineaAsistencia(ana, Presente: true)]);

        Assert.HasCount(1, await _e.Repositorio.ListarAsync<Asistencia>());
        Assert.AreEqual(NivelRiesgo.Bajo, (await _e.Seguimiento.PanelAsync(comision.Id))[0].Nivel);
    }

    [TestMethod]
    public async Task CA4_3_CorregirTambienPuedeJustificarUnaAusenciaDespues()
    {
        var comision = await _e.NuevaComisionAsync();
        var ana = await _e.NuevoInscriptoAsync(comision);
        var clase = await _e.NuevaClaseAsync(comision, -1);
        await _e.TomarAsistenciaAsync(clase, ana);

        await _e.Asistencias.CorregirAsync(clase.Id, [new LineaAsistencia(ana, Presente: false, Justificada: true)]);

        var planilla = await _e.Asistencias.ObtenerPlanillaAsync(clase.Id);
        Assert.AreEqual(EstadoAsistencia.AusenteJustificado, (await _e.Repositorio.ListarAsync<Asistencia>()).Single().Estado);
        Assert.IsTrue(planilla.Lineas[0].Justificada);
    }

    [TestMethod]
    public async Task NoSePuedeCorregirUnaClaseSinAsistenciaGuardada()
    {
        var comision = await _e.NuevaComisionAsync();
        var ana = await _e.NuevoInscriptoAsync(comision);
        var clase = await _e.NuevaClaseAsync(comision, -1);

        var ex = await Assert.ThrowsAsync<ReglaDeNegocioException>(
            () => _e.Asistencias.CorregirAsync(clase.Id, [new LineaAsistencia(ana)]));

        Assert.IsTrue(ex.Message.Contains("Todavía no hay asistencia"));
    }

    [TestMethod]
    public async Task NoSeGuardaAsistenciaDeUnEstudianteQueNoEstaActivoEnLaComision()
    {
        var comision = await _e.NuevaComisionAsync("Comisión A");
        var otra = await _e.NuevaComisionAsync("Comisión B");
        await _e.NuevoInscriptoAsync(comision);
        var ajeno = await _e.NuevoInscriptoAsync(otra, "Carla", "Díaz");
        var clase = await _e.NuevaClaseAsync(comision, -1);

        var ex = await Assert.ThrowsAsync<ReglaDeNegocioException>(
            () => _e.Asistencias.GuardarAsync(clase.Id, [new LineaAsistencia(ajeno)]));

        Assert.IsTrue(ex.Message.Contains("Carla Díaz"));
        Assert.IsEmpty(await _e.Repositorio.ListarAsync<Asistencia>());
    }

    [TestMethod]
    public async Task UnaClaseQueNoExisteSeRechaza()
    {
        var ex = await Assert.ThrowsAsync<ReglaDeNegocioException>(() => _e.Asistencias.ObtenerPlanillaAsync(999));

        Assert.IsTrue(ex.Message.Contains("clase"));
    }

    [TestMethod]
    public async Task UnEstudianteInscriptoDespuesDeTomarLaListaApareceComoPresenteAlCorregir()
    {
        var comision = await _e.NuevaComisionAsync();
        await _e.NuevoInscriptoAsync(comision, "Ana", "Gómez");
        var clase = await _e.NuevaClaseAsync(comision, -1);
        await _e.TomarAsistenciaAsync(clase);
        await _e.NuevoInscriptoAsync(comision, "Beto", "Pérez");

        var planilla = await _e.Asistencias.ObtenerPlanillaAsync(clase.Id);

        Assert.HasCount(2, planilla.Lineas);
        Assert.IsTrue(planilla.Lineas.All(l => l.Presente));
    }
}
