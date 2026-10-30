using App_v3.Dominio;

namespace App_v3.Tests;

[TestClass]
public sealed class SeguimientoServiceTests
{
    private readonly EscenarioDePrueba _e = new();

    private async Task<SituacionEstudiante> SituacionUnicaAsync(Comision comision)
    {
        var panel = await _e.Seguimiento.PanelAsync(comision.Id);
        Assert.HasCount(1, panel);
        return panel[0];
    }

    // ---------- H6: cálculo del riesgo ----------

    [TestMethod]
    public async Task CA6_1_SeCuentanLasInasistenciasYLosTpsVencidosSinEntregar()
    {
        var comision = await _e.NuevaComisionAsync();
        var ana = await _e.NuevoInscriptoAsync(comision);
        await _e.TomarAsistenciaAsync(await _e.NuevaClaseAsync(comision, -14), ana);
        await _e.TomarAsistenciaAsync(await _e.NuevaClaseAsync(comision, -7), ana);
        await _e.TomarAsistenciaAsync(await _e.NuevaClaseAsync(comision, -1));
        await _e.NuevoTrabajoAsync(comision, "TP 1", -5);

        var situacion = await SituacionUnicaAsync(comision);

        Assert.AreEqual(2, situacion.Inasistencias);
        Assert.AreEqual(1, situacion.TpsVencidosNoEntregados);
    }

    [TestMethod]
    [DataRow(0, 0, NivelRiesgo.Bajo)]
    [DataRow(2, 0, NivelRiesgo.Bajo)]
    [DataRow(3, 0, NivelRiesgo.Medio)]   // justo en el umbral Medio de inasistencias
    [DataRow(4, 0, NivelRiesgo.Medio)]
    [DataRow(5, 0, NivelRiesgo.Alto)]    // justo en el umbral Alto de inasistencias
    public async Task CA6_2_ElNivelPorInasistenciasRespetaLosUmbralesPorDefecto(int faltas, int tps, NivelRiesgo esperado)
    {
        var comision = await _e.NuevaComisionAsync();
        var ana = await _e.NuevoInscriptoAsync(comision);
        for (var i = 0; i < faltas; i++)
            await _e.TomarAsistenciaAsync(await _e.NuevaClaseAsync(comision, -(i + 1)), ana);

        Assert.AreEqual(esperado, (await SituacionUnicaAsync(comision)).Nivel);
        Assert.AreEqual(tps, (await SituacionUnicaAsync(comision)).TpsVencidosNoEntregados);
    }

    [TestMethod]
    [DataRow(0, NivelRiesgo.Bajo)]
    [DataRow(1, NivelRiesgo.Medio)]      // justo en el umbral Medio de TPs
    [DataRow(2, NivelRiesgo.Medio)]
    [DataRow(3, NivelRiesgo.Alto)]       // justo en el umbral Alto de TPs
    public async Task CA6_2_ElNivelPorTpsRespetaLosUmbralesPorDefecto(int tpsVencidos, NivelRiesgo esperado)
    {
        var comision = await _e.NuevaComisionAsync();
        await _e.NuevoInscriptoAsync(comision);
        for (var i = 0; i < tpsVencidos; i++)
            await _e.NuevoTrabajoAsync(comision, $"TP {i + 1}", -(i + 1));

        Assert.AreEqual(esperado, (await SituacionUnicaAsync(comision)).Nivel);
    }

    [TestMethod]
    public async Task CA6_2_ElNivelGeneralEsElMasAltoDeLasDosSenales()
    {
        var comision = await _e.NuevaComisionAsync();
        var ana = await _e.NuevoInscriptoAsync(comision);
        await _e.TomarAsistenciaAsync(await _e.NuevaClaseAsync(comision, -1), ana); // 1 falta: Bajo
        for (var i = 1; i <= 3; i++)
            await _e.NuevoTrabajoAsync(comision, $"TP {i}", -i);                    // 3 TPs: Alto

        Assert.AreEqual(NivelRiesgo.Alto, (await SituacionUnicaAsync(comision)).Nivel);
    }

    [TestMethod]
    public async Task UnTpQueTodaviaNoVencioNoCuentaComoNoEntregado()
    {
        var comision = await _e.NuevaComisionAsync();
        await _e.NuevoInscriptoAsync(comision);
        await _e.NuevoTrabajoAsync(comision, "TP hoy", 0);      // vence hoy: todavía está en plazo
        await _e.NuevoTrabajoAsync(comision, "TP futuro", 10);

        var situacion = await SituacionUnicaAsync(comision);

        Assert.AreEqual(0, situacion.TpsVencidosNoEntregados);
        Assert.AreEqual(NivelRiesgo.Bajo, situacion.Nivel);
    }

    [TestMethod]
    public async Task UnTpVencidoAyerSinRegistroDeEntregaCuentaComoNoEntregado()
    {
        var comision = await _e.NuevaComisionAsync();
        await _e.NuevoInscriptoAsync(comision);
        await _e.NuevoTrabajoAsync(comision, "TP 1", -1);

        Assert.AreEqual(1, (await SituacionUnicaAsync(comision)).TpsVencidosNoEntregados);
    }

    [TestMethod]
    public async Task UnTpVencidoEntregadoNoCuenta()
    {
        var comision = await _e.NuevaComisionAsync();
        var ana = await _e.NuevoInscriptoAsync(comision);
        var trabajo = await _e.NuevoTrabajoAsync(comision, "TP 1", -3);
        await _e.Entregas.GuardarAsync(trabajo.Id, [new LineaEntrega(ana, true)]);

        Assert.AreEqual(0, (await SituacionUnicaAsync(comision)).TpsVencidosNoEntregados);
    }

    [TestMethod]
    public async Task ElTranscurrirDelTiempoHaceQueUnTpPasadoAVencerSumeAlRiesgo()
    {
        var comision = await _e.NuevaComisionAsync();
        await _e.NuevoInscriptoAsync(comision);
        await _e.NuevoTrabajoAsync(comision, "TP 1", 2);
        Assert.AreEqual(NivelRiesgo.Bajo, (await SituacionUnicaAsync(comision)).Nivel);

        _e.Reloj.AvanzarDias(3);

        Assert.AreEqual(NivelRiesgo.Medio, (await SituacionUnicaAsync(comision)).Nivel);
    }

    [TestMethod]
    public async Task LasInasistenciasJustificadasNoCuentan()
    {
        var comision = await _e.NuevaComisionAsync();
        var ana = await _e.NuevoInscriptoAsync(comision);
        for (var i = 1; i <= 5; i++)
        {
            var clase = await _e.NuevaClaseAsync(comision, -i);
            await _e.Asistencias.GuardarAsync(clase.Id, [new LineaAsistencia(ana, Presente: false, Justificada: true)]);
        }

        var situacion = await SituacionUnicaAsync(comision);

        Assert.AreEqual(0, situacion.Inasistencias);
        Assert.AreEqual(NivelRiesgo.Bajo, situacion.Nivel);
    }

    [TestMethod]
    public async Task LasFaltasYLosTpsDeUnaComisionNoSumanEnLaOtra()
    {
        var comisionA = await _e.NuevaComisionAsync("Comisión A");
        var comisionB = await _e.NuevaComisionAsync("Comisión B");
        var ana = await _e.NuevoEstudianteAsync("L1", "30111222", "Ana", "Gómez");
        await _e.Inscripciones.InscribirAsync(ana.Id, comisionA.Id);
        await _e.Inscripciones.InscribirAsync(ana.Id, comisionB.Id);
        await _e.TomarAsistenciaAsync(await _e.NuevaClaseAsync(comisionA, -1), ana);
        await _e.NuevoTrabajoAsync(comisionA, "TP 1", -5);

        var enB = await SituacionUnicaAsync(comisionB);

        Assert.AreEqual(0, enB.Inasistencias);
        Assert.AreEqual(0, enB.TpsVencidosNoEntregados);
        Assert.AreEqual(1, (await SituacionUnicaAsync(comisionA)).Inasistencias);
    }

    // ---------- H6 CA 6.3 y H8 CA 8.4: el riesgo se actualiza ----------

    [TestMethod]
    public async Task CA6_3_AlModificarLosUmbralesCambiaElNivelDelEstudiante()
    {
        var comision = await _e.NuevaComisionAsync();
        var ana = await _e.NuevoInscriptoAsync(comision);
        await _e.TomarAsistenciaAsync(await _e.NuevaClaseAsync(comision, -1), ana);
        Assert.AreEqual(NivelRiesgo.Bajo, (await SituacionUnicaAsync(comision)).Nivel);

        await _e.Umbrales.GuardarAsync("1", "2", "1", "3");
        Assert.AreEqual(NivelRiesgo.Medio, (await SituacionUnicaAsync(comision)).Nivel);

        await _e.Umbrales.GuardarAsync("2", "3", "1", "3");
        Assert.AreEqual(NivelRiesgo.Bajo, (await SituacionUnicaAsync(comision)).Nivel);
    }

    // ---------- H8: panel ----------

    [TestMethod]
    public async Task CA8_1_ElPanelOrdenaDeAltoAMedioABajoYPorApellidoEnLosEmpates()
    {
        var comision = await _e.NuevaComisionAsync();
        var bajoZeta = await _e.NuevoInscriptoAsync(comision, "Zoe", "Zárate");
        var altoPerez = await _e.NuevoInscriptoAsync(comision, "Juan", "Pérez");
        var medio = await _e.NuevoInscriptoAsync(comision, "Mica", "Morales");
        var bajoAcosta = await _e.NuevoInscriptoAsync(comision, "Ana", "Acosta");
        var altoBravo = await _e.NuevoInscriptoAsync(comision, "Beto", "Bravo");
        for (var i = 1; i <= 5; i++)
            await _e.TomarAsistenciaAsync(await _e.NuevaClaseAsync(comision, -i), altoPerez, altoBravo);
        await _e.TomarAsistenciaAsync(await _e.NuevaClaseAsync(comision, -9), medio, medio, medio); // una sola falta
        await _e.Umbrales.GuardarAsync("1", "5", "1", "3");

        var panel = await _e.Seguimiento.PanelAsync(comision.Id);

        CollectionAssert(panel.Select(f => f.Estudiante.Id), altoBravo.Id, altoPerez.Id, medio.Id, bajoAcosta.Id, bajoZeta.Id);
        CollectionAssert(panel.Select(f => f.Nivel), NivelRiesgo.Alto, NivelRiesgo.Alto, NivelRiesgo.Medio, NivelRiesgo.Bajo, NivelRiesgo.Bajo);
    }

    private static void CollectionAssert<T>(IEnumerable<T> actual, params T[] esperado) =>
        Assert.AreEqual(string.Join(",", esperado), string.Join(",", actual));

    [TestMethod]
    public async Task CA8_2_CadaFilaTrae_InasistenciasTpsYElMotivo()
    {
        var comision = await _e.NuevaComisionAsync();
        var ana = await _e.NuevoInscriptoAsync(comision);
        await _e.TomarAsistenciaAsync(await _e.NuevaClaseAsync(comision, -2), ana);
        await _e.NuevoTrabajoAsync(comision, "TP 1", -1);
        await _e.NuevoTrabajoAsync(comision, "TP 2", -1);

        var situacion = await SituacionUnicaAsync(comision);

        Assert.AreEqual("1 inasistencia y 2 TPs vencidos sin entregar", situacion.Motivo);
    }

    [TestMethod]
    public async Task ElMotivoSinProblemasLoDiceAsi()
    {
        var comision = await _e.NuevaComisionAsync();
        await _e.NuevoInscriptoAsync(comision);

        Assert.AreEqual("Sin inasistencias ni TPs vencidos sin entregar", (await SituacionUnicaAsync(comision)).Motivo);
    }

    [TestMethod]
    public async Task ElMotivoUsaElSingularYElPlural()
    {
        var uno = new SituacionEstudiante(new Estudiante(), NivelRiesgo.Medio, 1, 1, null);
        var varios = new SituacionEstudiante(new Estudiante(), NivelRiesgo.Alto, 3, 0, null);

        Assert.AreEqual("1 inasistencia y 1 TP vencido sin entregar", uno.Motivo);
        Assert.AreEqual("3 inasistencias", varios.Motivo);
        await Task.CompletedTask;
    }

    [TestMethod]
    public async Task CA6_4_ElPanelNoIncluyeALosInactivosPeroSeConservaSuHistorial()
    {
        var comision = await _e.NuevaComisionAsync();
        var ana = await _e.NuevoInscriptoAsync(comision, "Ana", "Gómez");
        var beto = await _e.NuevoInscriptoAsync(comision, "Beto", "Pérez");
        await _e.TomarAsistenciaAsync(await _e.NuevaClaseAsync(comision, -1), beto);

        await _e.Inscripciones.DarDeBajaAsync(beto.Id, comision.Id, _e.Hoy);

        var panel = await _e.Seguimiento.PanelAsync(comision.Id);
        Assert.HasCount(1, panel);
        Assert.AreEqual(ana.Id, panel[0].Estudiante.Id);
        Assert.HasCount(2, await _e.Repositorio.ListarAsync<Asistencia>());
    }

    [TestMethod]
    public async Task UnaComisionSinEstudiantesTieneUnPanelVacio()
    {
        var comision = await _e.NuevaComisionAsync();

        Assert.IsEmpty(await _e.Seguimiento.PanelAsync(comision.Id));
    }

    [TestMethod]
    public async Task ElPanelDeUnaComisionQueNoExisteEstaVacio()
    {
        Assert.IsEmpty(await _e.Seguimiento.PanelAsync(12345));
    }

    // ---------- H11: contactos en el panel ----------

    [TestMethod]
    public async Task CA11_1_LaFilaMuestraLaFechaDelUltimoContacto()
    {
        var comision = await _e.NuevaComisionAsync();
        var ana = await _e.NuevoInscriptoAsync(comision);
        await _e.Contactos.RegistrarAsync(new DatosContacto(ana.Id, comision.Id, _e.Hoy.AddDays(-9), MedioContacto.Llamada, "Respondió", "Habló"));
        await _e.Contactos.RegistrarAsync(new DatosContacto(ana.Id, comision.Id, _e.Hoy.AddDays(-2), MedioContacto.Email, "No respondió", "Mail"));

        Assert.AreEqual(_e.Hoy.AddDays(-2), (await SituacionUnicaAsync(comision)).UltimoContacto);
    }

    [TestMethod]
    public async Task CA11_1_SinContactosElUltimoContactoEsNulo()
    {
        var comision = await _e.NuevaComisionAsync();
        await _e.NuevoInscriptoAsync(comision);

        Assert.IsNull((await SituacionUnicaAsync(comision)).UltimoContacto);
    }

    [TestMethod]
    public async Task CA11_1_UnContactoEnOtraComisionNoAparece()
    {
        var comisionA = await _e.NuevaComisionAsync("Comisión A");
        var comisionB = await _e.NuevaComisionAsync("Comisión B");
        var ana = await _e.NuevoEstudianteAsync("L1", "30111222", "Ana", "Gómez");
        await _e.Inscripciones.InscribirAsync(ana.Id, comisionA.Id);
        await _e.Inscripciones.InscribirAsync(ana.Id, comisionB.Id);
        await _e.Contactos.RegistrarAsync(new DatosContacto(ana.Id, comisionA.Id, _e.Hoy, MedioContacto.Llamada, "Respondió", "Habló"));

        Assert.IsNull((await SituacionUnicaAsync(comisionB)).UltimoContacto);
        Assert.IsNotNull((await SituacionUnicaAsync(comisionA)).UltimoContacto);
    }

    [TestMethod]
    public async Task CA11_2_ElFiltroMuestraSoloEnRiesgoSinContactoEnLosUltimos7Dias()
    {
        var comision = await _e.NuevaComisionAsync();
        var sinContacto = await _e.NuevoInscriptoAsync(comision, "Ana", "Acosta");
        var contactadoHoy = await _e.NuevoInscriptoAsync(comision, "Beto", "Bravo");
        var contactoViejo = await _e.NuevoInscriptoAsync(comision, "Carla", "Cruz");
        var sinRiesgo = await _e.NuevoInscriptoAsync(comision, "Dani", "Díaz");
        await _e.Umbrales.GuardarAsync("1", "5", "1", "3");
        await _e.TomarAsistenciaAsync(await _e.NuevaClaseAsync(comision, -1), sinContacto, contactadoHoy, contactoViejo);
        await _e.Contactos.RegistrarAsync(new DatosContacto(contactadoHoy.Id, comision.Id, _e.Hoy, MedioContacto.Llamada, "Respondió", "ok"));
        await _e.Contactos.RegistrarAsync(new DatosContacto(contactoViejo.Id, comision.Id, _e.Hoy.AddDays(-8), MedioContacto.Email, "No respondió", "ok"));

        var filtrados = await _e.Seguimiento.PanelAsync(comision.Id, soloNoContactadosEstaSemana: true);

        CollectionAssert(filtrados.Select(f => f.Estudiante.Id), sinContacto.Id, contactoViejo.Id);
        Assert.IsFalse(filtrados.Any(f => f.Estudiante.Id == sinRiesgo.Id));
    }

    [TestMethod]
    public async Task CA11_2_UnContactoDeHace7DiasTodavíaCuentaComoDeEstaSemana()
    {
        var comision = await _e.NuevaComisionAsync();
        var ana = await _e.NuevoInscriptoAsync(comision);
        await _e.Umbrales.GuardarAsync("1", "5", "1", "3");
        await _e.TomarAsistenciaAsync(await _e.NuevaClaseAsync(comision, -20), ana);
        await _e.Contactos.RegistrarAsync(new DatosContacto(ana.Id, comision.Id, _e.Hoy.AddDays(-7), MedioContacto.Llamada, "Respondió", "ok"));

        Assert.IsEmpty(await _e.Seguimiento.PanelAsync(comision.Id, soloNoContactadosEstaSemana: true));

        _e.Reloj.AvanzarDias(1);
        Assert.HasCount(1, await _e.Seguimiento.PanelAsync(comision.Id, soloNoContactadosEstaSemana: true));
    }

    // ---------- H9, H12 y H13: ficha e historial ----------

    [TestMethod]
    public async Task CA9_1_LaFichaTraeLosDatosPersonalesDelEstudiante()
    {
        var comision = await _e.NuevaComisionAsync();
        var estudiante = await _e.Estudiantes.AltaAsync(
            new DatosEstudiante("L-77", "30.123.456", "Ana", "Gómez", "ana@mail.com", "11-5555-0000"));
        await _e.Inscripciones.InscribirAsync(estudiante.Id, comision.Id);

        var ficha = await _e.Seguimiento.ObtenerFichaAsync(estudiante.Id, comision.Id);

        Assert.AreEqual("L-77", ficha.Estudiante.Legajo);
        Assert.AreEqual("30123456", ficha.Estudiante.Dni);
        Assert.AreEqual("Ana Gómez", ficha.Estudiante.NombreCompleto);
        Assert.AreEqual("ana@mail.com", ficha.Estudiante.Email);
        Assert.AreEqual("11-5555-0000", ficha.Estudiante.Telefono);
    }

    [TestMethod]
    public async Task CA9_2_YCA9_3_LaFichaMuestraNivelMotivoYEstadoActivo()
    {
        var comision = await _e.NuevaComisionAsync();
        var ana = await _e.NuevoInscriptoAsync(comision);
        await _e.NuevoTrabajoAsync(comision, "TP 1", -1);

        var ficha = await _e.Seguimiento.ObtenerFichaAsync(ana.Id, comision.Id);

        Assert.IsTrue(ficha.Activo);
        Assert.AreEqual(NivelRiesgo.Medio, ficha.Nivel);
        Assert.AreEqual("1 TP vencido sin entregar", ficha.Motivo);
        Assert.IsNull(ficha.FechaBaja);
    }

    [TestMethod]
    public async Task CA12_1_ElHistorialListaLasClasesRegistradasConSuEstado()
    {
        var comision = await _e.NuevaComisionAsync();
        var ana = await _e.NuevoInscriptoAsync(comision);
        await _e.TomarAsistenciaAsync(await _e.NuevaClaseAsync(comision, -14), ana);
        var segunda = await _e.NuevaClaseAsync(comision, -7);
        await _e.Asistencias.GuardarAsync(segunda.Id, [new LineaAsistencia(ana, false, true)]);
        await _e.TomarAsistenciaAsync(await _e.NuevaClaseAsync(comision, -1));
        await _e.NuevaClaseAsync(comision, 7); // futura, sin registro: no aparece

        var ficha = await _e.Seguimiento.ObtenerFichaAsync(ana.Id, comision.Id);

        CollectionAssert(ficha.Asistencias.Select(a => a.Estado),
            EstadoAsistencia.Ausente, EstadoAsistencia.AusenteJustificado, EstadoAsistencia.Presente);
        CollectionAssert(ficha.Asistencias.Select(a => a.Clase.Numero), 1, 2, 3);
    }

    [TestMethod]
    public async Task CA12_2_ElHistorialListaLosTrabajosConEntregadoNoEntregadoOPendiente()
    {
        var comision = await _e.NuevaComisionAsync();
        var ana = await _e.NuevoInscriptoAsync(comision);
        var entregado = await _e.NuevoTrabajoAsync(comision, "TP 1", -20);
        await _e.NuevoTrabajoAsync(comision, "TP 2", -5);
        await _e.NuevoTrabajoAsync(comision, "TP 3", 10);
        await _e.Entregas.GuardarAsync(entregado.Id, [new LineaEntrega(ana, true)]);

        var ficha = await _e.Seguimiento.ObtenerFichaAsync(ana.Id, comision.Id);

        CollectionAssert(ficha.Entregas.Select(e => e.Estado),
            EstadoEntrega.Entregado, EstadoEntrega.NoEntregado, EstadoEntrega.Pendiente);
    }

    [TestMethod]
    public async Task CA12_3_ElResumenTieneElTotalDeInasistenciasYDeTpsNoEntregados()
    {
        var comision = await _e.NuevaComisionAsync();
        var ana = await _e.NuevoInscriptoAsync(comision);
        await _e.TomarAsistenciaAsync(await _e.NuevaClaseAsync(comision, -3), ana);
        await _e.TomarAsistenciaAsync(await _e.NuevaClaseAsync(comision, -2), ana);
        await _e.TomarAsistenciaAsync(await _e.NuevaClaseAsync(comision, -1));
        await _e.NuevoTrabajoAsync(comision, "TP 1", -5);
        await _e.NuevoTrabajoAsync(comision, "TP 2", 5);

        var ficha = await _e.Seguimiento.ObtenerFichaAsync(ana.Id, comision.Id);

        Assert.AreEqual(2, ficha.TotalInasistencias);
        Assert.AreEqual(1, ficha.TotalTpsNoEntregados);
    }

    [TestMethod]
    public async Task CA10_1_LaFichaIncluyeElHistorialDeContactos()
    {
        var comision = await _e.NuevaComisionAsync();
        var ana = await _e.NuevoInscriptoAsync(comision);
        await _e.Contactos.RegistrarAsync(new DatosContacto(ana.Id, comision.Id, _e.Hoy.AddDays(-5), MedioContacto.Llamada, "Respondió", "Primero"));
        await _e.Contactos.RegistrarAsync(new DatosContacto(ana.Id, comision.Id, _e.Hoy.AddDays(-1), MedioContacto.Email, "No respondió", "Segundo"));

        var ficha = await _e.Seguimiento.ObtenerFichaAsync(ana.Id, comision.Id);

        CollectionAssert(ficha.Contactos.Select(c => c.Nota), "Segundo", "Primero");
    }

    [TestMethod]
    public async Task CA12_4_YCA13_2_LaFichaDeUnInactivoConservaTodoSuHistorial()
    {
        var comision = await _e.NuevaComisionAsync();
        var beto = await _e.NuevoInscriptoAsync(comision, "Beto", "Pérez");
        await _e.TomarAsistenciaAsync(await _e.NuevaClaseAsync(comision, -7), beto);
        var trabajo = await _e.NuevoTrabajoAsync(comision, "TP 1", -3);
        await _e.Entregas.GuardarAsync(trabajo.Id, [new LineaEntrega(beto, true)]);
        await _e.Inscripciones.DarDeBajaAsync(beto.Id, comision.Id, _e.Hoy);

        var ficha = await _e.Seguimiento.ObtenerFichaAsync(beto.Id, comision.Id);

        Assert.IsFalse(ficha.Activo);
        Assert.AreEqual(_e.Hoy, ficha.FechaBaja);
        Assert.IsNull(ficha.Nivel);
        Assert.AreEqual("Beto Pérez", ficha.Estudiante.NombreCompleto);
        Assert.HasCount(1, ficha.Asistencias);
        Assert.AreEqual(1, ficha.TotalInasistencias);
        Assert.AreEqual(EstadoEntrega.Entregado, ficha.Entregas[0].Estado);
    }

    [TestMethod]
    public async Task LaFichaDeAlguienQueNoEstaInscriptoEnLaComisionSeRechaza()
    {
        var comision = await _e.NuevaComisionAsync();
        var otro = await _e.NuevoEstudianteAsync();

        var ex = await Assert.ThrowsAsync<ReglaDeNegocioException>(
            () => _e.Seguimiento.ObtenerFichaAsync(otro.Id, comision.Id));

        Assert.IsTrue(ex.Message.Contains("no está inscripto"));
    }
}
