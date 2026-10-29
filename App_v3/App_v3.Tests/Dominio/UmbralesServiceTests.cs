using App_v3.Dominio;

namespace App_v3.Tests;

[TestClass]
public sealed class UmbralesServiceTests
{
    private readonly EscenarioDePrueba _e = new();

    [TestMethod]
    public async Task SiNadieLosConfiguroSeUsanLosUmbralesPorDefecto()
    {
        var umbrales = await _e.Umbrales.ObtenerAsync();

        Assert.AreEqual(3, umbrales.InasistenciasMedio);
        Assert.AreEqual(5, umbrales.InasistenciasAlto);
        Assert.AreEqual(1, umbrales.TpsNoEntregadosMedio);
        Assert.AreEqual(3, umbrales.TpsNoEntregadosAlto);
    }

    [TestMethod]
    public async Task CA7_1_GuardarRegistraLaNuevaConfiguracion()
    {
        await _e.Umbrales.GuardarAsync("2", "4", "2", "5");

        var leidos = await _e.Umbrales.ObtenerAsync();
        Assert.AreEqual(2, leidos.InasistenciasMedio);
        Assert.AreEqual(4, leidos.InasistenciasAlto);
        Assert.AreEqual(2, leidos.TpsNoEntregadosMedio);
        Assert.AreEqual(5, leidos.TpsNoEntregadosAlto);
    }

    [TestMethod]
    public async Task CA7_1_GuardarVariasVecesActualizaLaMismaConfiguracion()
    {
        await _e.Umbrales.GuardarAsync("2", "4", "2", "5");
        await _e.Umbrales.GuardarAsync("3", "6", "1", "4");

        Assert.HasCount(1, await _e.Repositorio.ListarAsync<UmbralesRiesgo>());
        Assert.AreEqual(6, (await _e.Umbrales.ObtenerAsync()).InasistenciasAlto);
    }

    [TestMethod]
    public async Task CA7_1_AceptaEspaciosAlrededorDelNumero()
    {
        var guardados = await _e.Umbrales.GuardarAsync(" 2 ", " 4", "2 ", "5");

        Assert.AreEqual(2, guardados.InasistenciasMedio);
    }

    [TestMethod]
    [DataRow("abc", "5", "1", "3")]
    [DataRow("3", "", "1", "3")]
    [DataRow("3", "5", null, "3")]
    [DataRow("3", "5", "1", "2.5")]
    [DataRow("3", "5", "1", "tres")]
    public async Task CA7_3_UnValorQueNoEsUnEnteroNoSeGuardaNiDiceQueSeActualizo(
        string? inasistenciasMedio, string? inasistenciasAlto, string? tpsMedio, string? tpsAlto)
    {
        var ex = await Assert.ThrowsAsync<ReglaDeNegocioException>(
            () => _e.Umbrales.GuardarAsync(inasistenciasMedio, inasistenciasAlto, tpsMedio, tpsAlto));

        Assert.IsTrue(ex.Message.Contains("números enteros"));
        Assert.IsEmpty(await _e.Repositorio.ListarAsync<UmbralesRiesgo>());
    }

    [TestMethod]
    public async Task CA7_3_LosValoresContradictoriosNoSeGuardanYSeConservanLosAnteriores()
    {
        await _e.Umbrales.GuardarAsync("2", "4", "2", "5");

        var ex = await Assert.ThrowsAsync<ReglaDeNegocioException>(
            () => _e.Umbrales.GuardarAsync("6", "3", "2", "5"));

        Assert.AreEqual("El umbral Medio de inasistencias debe ser menor que el Alto.", ex.Message);
        Assert.AreEqual(4, (await _e.Umbrales.ObtenerAsync()).InasistenciasAlto);
    }

    [TestMethod]
    public async Task CA7_2_LosNuevosUmbralesCambianElNivelCalculado()
    {
        var antes = (await _e.Umbrales.ObtenerAsync()).CalcularNivel(inasistencias: 3, tpsNoEntregados: 0);

        await _e.Umbrales.GuardarAsync("1", "3", "1", "3");
        var despues = (await _e.Umbrales.ObtenerAsync()).CalcularNivel(inasistencias: 3, tpsNoEntregados: 0);

        Assert.AreEqual(NivelRiesgo.Medio, antes);
        Assert.AreEqual(NivelRiesgo.Alto, despues);
    }
}
