using App_v3.Models;

namespace App_v3.Tests;

[TestClass]
public sealed class ConfiguracionRiesgoTests
{
    [TestMethod]
    public void CA7_1_UmbralesPorDefectoSonValidos()
    {
        var config = new ConfiguracionRiesgo();

        Assert.IsTrue(config.EsValida(out var mensaje));
        Assert.AreEqual(string.Empty, mensaje);
    }

    [TestMethod]
    public void CA7_1_UmbralesPersonalizadosCoherentesSonValidos()
    {
        var config = new ConfiguracionRiesgo
        {
            InasistenciasMedio = 2,
            InasistenciasAlto = 4,
            TpsNoEntregadosMedio = 2,
            TpsNoEntregadosAlto = 5
        };

        Assert.IsTrue(config.EsValida(out _));
    }

    [TestMethod]
    public void CA7_2_PocasInasistenciasYSinTpsPendientesDaRiesgoBajo()
    {
        var config = new ConfiguracionRiesgo();

        var nivel = config.CalcularNivel(inasistencias: 1, tpsNoEntregados: 0);

        Assert.AreEqual(NivelRiesgo.Bajo, nivel);
    }

    [TestMethod]
    public void CA7_2_MuchasInasistenciasDaRiesgoAlto()
    {
        var config = new ConfiguracionRiesgo();

        var nivel = config.CalcularNivel(inasistencias: 5, tpsNoEntregados: 0);

        Assert.AreEqual(NivelRiesgo.Alto, nivel);
    }

    [TestMethod]
    public void CA7_2_ElNivelGeneralEsElMasAltoDeLasDosSenales()
    {
        var config = new ConfiguracionRiesgo();

        // 2 inasistencias es Bajo, pero 1 TP sin entregar es Medio
        var nivel = config.CalcularNivel(inasistencias: 2, tpsNoEntregados: 1);

        Assert.AreEqual(NivelRiesgo.Medio, nivel);
    }

    [TestMethod]
    public void CA7_2_AlBajarElUmbralAltoCambiaElNivelDelEstudiante()
    {
        var config = new ConfiguracionRiesgo();
        var antes = config.CalcularNivel(inasistencias: 4, tpsNoEntregados: 0);

        config.InasistenciasAlto = 4;
        var despues = config.CalcularNivel(inasistencias: 4, tpsNoEntregados: 0);

        Assert.AreEqual(NivelRiesgo.Medio, antes);
        Assert.AreEqual(NivelRiesgo.Alto, despues);
    }

    [TestMethod]
    [DataRow(5, 3, 1, 3)]   // inasistencias: Medio mayor que Alto
    [DataRow(4, 4, 1, 3)]   // inasistencias: Medio igual a Alto
    [DataRow(3, 5, 3, 2)]   // TPs: Medio mayor que Alto
    [DataRow(3, 5, 2, 2)]   // TPs: Medio igual a Alto
    [DataRow(0, 5, 1, 3)]   // umbral menor a 1
    public void CA7_3_RechazaValoresContradictorios(
        int inasistenciasMedio, int inasistenciasAlto, int tpsMedio, int tpsAlto)
    {
        var config = new ConfiguracionRiesgo
        {
            InasistenciasMedio = inasistenciasMedio,
            InasistenciasAlto = inasistenciasAlto,
            TpsNoEntregadosMedio = tpsMedio,
            TpsNoEntregadosAlto = tpsAlto
        };

        Assert.IsFalse(config.EsValida(out var mensaje));
        Assert.IsFalse(string.IsNullOrEmpty(mensaje));
    }

    [TestMethod]
    public void CA7_3_InformaQueElUmbralMedioDebeSerMenorQueElAlto()
    {
        var config = new ConfiguracionRiesgo { InasistenciasMedio = 6, InasistenciasAlto = 3 };

        Assert.IsFalse(config.EsValida(out var mensaje));
        Assert.AreEqual("El umbral Medio de inasistencias debe ser menor que el Alto.", mensaje);
    }
}
