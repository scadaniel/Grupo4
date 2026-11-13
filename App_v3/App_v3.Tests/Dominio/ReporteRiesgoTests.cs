using App_v3.Dominio;

namespace App_v3.Tests;

/// <summary>H12: reporte de estudiantes en riesgo de la comisión (CA1, CA2 y CA3).</summary>
[TestClass]
public sealed class ReporteRiesgoTests
{
    private readonly EscenarioDePrueba _e = new();

    // Comisión con 5 estudiantes y 3 clases ya dictadas. Los dos primeros faltaron a las tres:
    // 3 inasistencias es riesgo Medio con los umbrales por defecto.
    private async Task<Comision> ComisionConDosEnRiesgoAsync()
    {
        var comision = await _e.NuevaComisionAsync();
        var estudiantes = new List<Estudiante>();
        for (var i = 0; i < 5; i++)
            estudiantes.Add(await _e.NuevoInscriptoAsync(comision));

        foreach (var dias in new[] { -21, -14, -7 })
            await _e.TomarAsistenciaAsync(await _e.NuevaClaseAsync(comision, dias), estudiantes[0], estudiantes[1]);

        return comision;
    }

    // ---------- CA1 · El número ----------

    [TestMethod]
    public async Task CA1_ElReporteDiceCuantosEstanEnRiesgoDeCuantos()
    {
        var comision = await ComisionConDosEnRiesgoAsync();

        var resumen = await _e.Seguimiento.ResumenDeRiesgoAsync(comision.Id);

        Assert.AreEqual(EstadoResumenRiesgo.Calculado, resumen.Estado);
        Assert.AreEqual(2, resumen.EnRiesgo);
        Assert.AreEqual(5, resumen.TotalActivos);
        Assert.AreEqual(2, resumen.Medio);
        Assert.AreEqual(0, resumen.Alto);
    }

    // ---------- CA2 · Coincide con el panel ----------

    [TestMethod]
    public async Task CA2_ElReporteCoincideConElPanel()
    {
        var comision = await ComisionConDosEnRiesgoAsync();

        var resumen = await _e.Seguimiento.ResumenDeRiesgoAsync(comision.Id);
        var panel = await _e.Seguimiento.PanelAsync(comision.Id);

        Assert.AreEqual(panel.Count(f => f.Nivel != NivelRiesgo.Bajo), resumen.EnRiesgo);
        Assert.AreEqual(panel.Count, resumen.TotalActivos);
    }

    [TestMethod]
    public async Task CA2_UnCambioDeUmbralesCambiaElPanelYElReporteJuntos()
    {
        var comision = await ComisionConDosEnRiesgoAsync();

        // Con 4 inasistencias como umbral Medio, las 3 faltas ya no alcanzan.
        await _e.Umbrales.GuardarAsync("4", "5", "1", "3");

        var resumen = await _e.Seguimiento.ResumenDeRiesgoAsync(comision.Id);
        var panel = await _e.Seguimiento.PanelAsync(comision.Id);

        Assert.AreEqual(0, resumen.EnRiesgo);
        Assert.AreEqual(panel.Count(f => f.Nivel != NivelRiesgo.Bajo), resumen.EnRiesgo);
        Assert.AreEqual(EstadoResumenRiesgo.Calculado, resumen.Estado);
    }

    [TestMethod]
    public async Task ElReporteNoCuentaALosEstudiantesDadosDeBaja()
    {
        var comision = await ComisionConDosEnRiesgoAsync();
        var activos = await _e.Inscripciones.ListarActivosAsync(comision.Id);
        await _e.Inscripciones.DarDeBajaAsync(activos[0].Id, comision.Id, _e.Hoy);

        var resumen = await _e.Seguimiento.ResumenDeRiesgoAsync(comision.Id);

        Assert.AreEqual(4, resumen.TotalActivos);
    }

    // ---------- CA3 · Comisión sin datos ----------

    [TestMethod]
    public async Task CA3_ConEstudiantesPeroSinAsistenciasNiEntregasInformaQueNoHayDatos()
    {
        var comision = await _e.NuevaComisionAsync();
        for (var i = 0; i < 3; i++) await _e.NuevoInscriptoAsync(comision);

        var resumen = await _e.Seguimiento.ResumenDeRiesgoAsync(comision.Id);

        Assert.AreEqual(EstadoResumenRiesgo.SinDatos, resumen.Estado);
        Assert.AreEqual(3, resumen.TotalActivos);
        Assert.AreEqual(0, resumen.EnRiesgo);
    }

    [TestMethod]
    public async Task CA3_UnaClaseCargadaSinAsistenciaTomadaNoEsUnDato()
    {
        var comision = await _e.NuevaComisionAsync();
        await _e.NuevoInscriptoAsync(comision);
        await _e.NuevaClaseAsync(comision, -7);

        var resumen = await _e.Seguimiento.ResumenDeRiesgoAsync(comision.Id);

        Assert.AreEqual(EstadoResumenRiesgo.SinDatos, resumen.Estado);
    }

    [TestMethod]
    public async Task CA3_ConDatosYNadieEnRiesgoDiceCeroPeroCalculado()
    {
        var comision = await _e.NuevaComisionAsync();
        await _e.NuevoInscriptoAsync(comision);
        await _e.TomarAsistenciaAsync(await _e.NuevaClaseAsync(comision, -7));

        var resumen = await _e.Seguimiento.ResumenDeRiesgoAsync(comision.Id);

        // Este sí es "0 en riesgo": hay datos y todo está bien. No es lo mismo que "sin datos".
        Assert.AreEqual(EstadoResumenRiesgo.Calculado, resumen.Estado);
        Assert.AreEqual(0, resumen.EnRiesgo);
        Assert.AreEqual(1, resumen.TotalActivos);
    }

    [TestMethod]
    public async Task UnaComisionSinEstudiantesActivosTieneSuPropioEstado()
    {
        var comision = await _e.NuevaComisionAsync();

        var resumen = await _e.Seguimiento.ResumenDeRiesgoAsync(comision.Id);

        Assert.AreEqual(EstadoResumenRiesgo.SinEstudiantes, resumen.Estado);
    }

    // ---------- Con los datos de ejemplo de la demo ----------

    [TestMethod]
    public async Task LosDatosDeEjemploDan7De30En1AYSinDatosEn1B()
    {
        await new DatosDeEjemplo(_e.Repositorio, _e.Reloj).CargarAsync();
        var comisiones = await _e.Comisiones.ListarAsync();
        var a = comisiones.Single(c => c.Nombre == "1°A");
        var b = comisiones.Single(c => c.Nombre == "1°B");

        var resumenA = await _e.Seguimiento.ResumenDeRiesgoAsync(a.Id);
        var panelA = await _e.Seguimiento.PanelAsync(a.Id);
        var resumenB = await _e.Seguimiento.ResumenDeRiesgoAsync(b.Id);

        Assert.AreEqual(EstadoResumenRiesgo.Calculado, resumenA.Estado);
        Assert.AreEqual(7, resumenA.EnRiesgo);
        Assert.AreEqual(30, resumenA.TotalActivos);
        Assert.AreEqual(7, panelA.Count(f => f.Nivel != NivelRiesgo.Bajo));
        Assert.AreEqual(EstadoResumenRiesgo.SinDatos, resumenB.Estado);
        Assert.AreEqual(30, resumenB.TotalActivos);
    }
}
