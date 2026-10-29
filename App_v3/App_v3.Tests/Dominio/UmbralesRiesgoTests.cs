using App_v3.Dominio;

namespace App_v3.Tests;

[TestClass]
public sealed class UmbralesRiesgoTests
{
    [TestMethod]
    public void CA7_1_UmbralesPorDefectoSonValidos()
    {
        var umbrales = new UmbralesRiesgo();

        Assert.IsTrue(umbrales.EsValida(out var mensaje));
        Assert.AreEqual(string.Empty, mensaje);
    }

    [TestMethod]
    public void CA7_1_UmbralesPersonalizadosCoherentesSonValidos()
    {
        var umbrales = new UmbralesRiesgo
        {
            InasistenciasMedio = 2,
            InasistenciasAlto = 4,
            TpsNoEntregadosMedio = 2,
            TpsNoEntregadosAlto = 5
        };

        Assert.IsTrue(umbrales.EsValida(out _));
    }

    [TestMethod]
    public void CA6_2_PocasInasistenciasYSinTpsPendientesDaRiesgoBajo()
    {
        var nivel = new UmbralesRiesgo().CalcularNivel(inasistencias: 1, tpsNoEntregados: 0);

        Assert.AreEqual(NivelRiesgo.Bajo, nivel);
    }

    [TestMethod]
    public void CA6_2_MuchasInasistenciasDaRiesgoAlto()
    {
        var nivel = new UmbralesRiesgo().CalcularNivel(inasistencias: 5, tpsNoEntregados: 0);

        Assert.AreEqual(NivelRiesgo.Alto, nivel);
    }

    [TestMethod]
    public void CA6_2_ElNivelGeneralEsElMasAltoDeLasDosSenales()
    {
        // 2 inasistencias es Bajo, pero 1 TP sin entregar es Medio
        var nivel = new UmbralesRiesgo().CalcularNivel(inasistencias: 2, tpsNoEntregados: 1);

        Assert.AreEqual(NivelRiesgo.Medio, nivel);
    }

    [TestMethod]
    [DataRow(0, 0, NivelRiesgo.Bajo)]
    [DataRow(2, 0, NivelRiesgo.Bajo)]
    [DataRow(3, 0, NivelRiesgo.Medio)]   // justo en el umbral Medio de inasistencias
    [DataRow(4, 0, NivelRiesgo.Medio)]
    [DataRow(5, 0, NivelRiesgo.Alto)]    // justo en el umbral Alto de inasistencias
    [DataRow(0, 1, NivelRiesgo.Medio)]   // justo en el umbral Medio de TPs
    [DataRow(0, 2, NivelRiesgo.Medio)]
    [DataRow(0, 3, NivelRiesgo.Alto)]    // justo en el umbral Alto de TPs
    [DataRow(5, 3, NivelRiesgo.Alto)]
    public void CA6_2_CasosBordeDeLosUmbralesPorDefecto(int inasistencias, int tps, NivelRiesgo esperado)
    {
        Assert.AreEqual(esperado, new UmbralesRiesgo().CalcularNivel(inasistencias, tps));
    }

    [TestMethod]
    public void CA7_2_AlBajarElUmbralAltoCambiaElNivelDelEstudiante()
    {
        var umbrales = new UmbralesRiesgo();
        var antes = umbrales.CalcularNivel(inasistencias: 4, tpsNoEntregados: 0);

        umbrales.InasistenciasAlto = 4;
        var despues = umbrales.CalcularNivel(inasistencias: 4, tpsNoEntregados: 0);

        Assert.AreEqual(NivelRiesgo.Medio, antes);
        Assert.AreEqual(NivelRiesgo.Alto, despues);
    }

    [TestMethod]
    [DataRow(5, 3, 1, 3)]   // inasistencias: Medio mayor que Alto
    [DataRow(4, 4, 1, 3)]   // inasistencias: Medio igual a Alto
    [DataRow(3, 5, 3, 2)]   // TPs: Medio mayor que Alto
    [DataRow(3, 5, 2, 2)]   // TPs: Medio igual a Alto
    [DataRow(0, 5, 1, 3)]   // umbral menor a 1
    [DataRow(3, 5, -1, 3)]  // umbral negativo
    public void CA7_3_RechazaValoresContradictorios(
        int inasistenciasMedio, int inasistenciasAlto, int tpsMedio, int tpsAlto)
    {
        var umbrales = new UmbralesRiesgo
        {
            InasistenciasMedio = inasistenciasMedio,
            InasistenciasAlto = inasistenciasAlto,
            TpsNoEntregadosMedio = tpsMedio,
            TpsNoEntregadosAlto = tpsAlto
        };

        Assert.IsFalse(umbrales.EsValida(out var mensaje));
        Assert.IsFalse(string.IsNullOrEmpty(mensaje));
    }

    [TestMethod]
    public void CA7_3_InformaQueElUmbralMedioDebeSerMenorQueElAlto()
    {
        var umbrales = new UmbralesRiesgo { InasistenciasMedio = 6, InasistenciasAlto = 3 };

        Assert.IsFalse(umbrales.EsValida(out var mensaje));
        Assert.AreEqual("El umbral Medio de inasistencias debe ser menor que el Alto.", mensaje);
    }
}
