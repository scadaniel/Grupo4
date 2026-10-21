using System.Globalization;
using App_v3.Models;

namespace App_v3.Tests;

[TestClass]
public sealed class ReporteAsistenciaItemTests
{
    [TestMethod]
    public void CA12_1_PresenteSeMuestraEnVerde()
    {
        var item = new ReporteAsistenciaItem { Presente = true };

        Assert.AreEqual("Presente", item.EstadoTexto);
        Assert.AreEqual("#2E7D32", item.ColorEstado);
    }

    [TestMethod]
    public void CA12_1_AusenteSinJustificarSeMuestraEnRojo()
    {
        var item = new ReporteAsistenciaItem { Presente = false, Justificada = false };

        Assert.AreEqual("Ausente", item.EstadoTexto);
        Assert.AreEqual("#D32F2F", item.ColorEstado);
    }

    [TestMethod]
    public void CA4_2_AusenteJustificadoSeMuestraEnNaranja()
    {
        var item = new ReporteAsistenciaItem { Presente = false, Justificada = true };

        Assert.AreEqual("Justificado", item.EstadoTexto);
        Assert.AreEqual("#F57C00", item.ColorEstado);
    }

    [TestMethod]
    public void CA12_1_SiEstuvoPresenteLaJustificacionNoCambiaElEstado()
    {
        var item = new ReporteAsistenciaItem { Presente = true, Justificada = true };

        Assert.AreEqual("Presente", item.EstadoTexto);
    }
}

[TestClass]
public sealed class ReporteTpItemTests
{
    [TestMethod]
    public void CA12_2_TrabajoEntregadoSeMuestraEnVerde()
    {
        var item = new ReporteTpItem { Entregado = true };

        Assert.AreEqual("Entregado", item.EstadoTexto);
        Assert.AreEqual("#2E7D32", item.ColorEstado);
    }

    [TestMethod]
    public void CA12_2_TrabajoNoEntregadoQuedaPendienteEnRojo()
    {
        var item = new ReporteTpItem { Entregado = false };

        Assert.AreEqual("Pendiente", item.EstadoTexto);
        Assert.AreEqual("#D32F2F", item.ColorEstado);
    }

    [TestMethod]
    public void CA12_2_SinEntregaRegistradaLoIndicaEnElTexto()
    {
        var item = new ReporteTpItem { Entregado = false, FechaEntrega = null };

        Assert.AreEqual("Sin entrega registrada", item.TextoFechaEntrega);
    }

    [TestMethod]
    public void CA12_2_ConEntregaRegistradaMuestraLaFecha()
    {
        var item = new ReporteTpItem { Entregado = true, FechaEntrega = new DateTime(2026, 10, 5) };
        var original = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            Assert.AreEqual("Entregado el: 05/10/2026", item.TextoFechaEntrega);
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }
    }
}

[TestClass]
public sealed class ModelosPorDefectoTests
{
    [TestMethod]
    public void CA4_2_UnaAsistenciaNuevaNoEstaJustificada()
    {
        var asistencia = new Asistencia();

        Assert.IsFalse(asistencia.Justificada);
    }

    [TestMethod]
    public void CA5_2_UnaEntregaNuevaQuedaPendienteYSinFecha()
    {
        var entrega = new EntregaTp();

        Assert.IsFalse(entrega.Entregado);
        Assert.IsNull(entrega.FechaEntrega);
    }
}

[TestClass]
public sealed class RiesgoAcademicoTests
{
    [TestMethod]
    [DataRow(6.0, 75, false)]   // justo en el límite: no está en riesgo
    [DataRow(10.0, 100, false)]
    [DataRow(5.9, 75, true)]    // promedio apenas por debajo
    [DataRow(6.0, 74, true)]    // asistencia apenas por debajo
    [DataRow(4.0, 50, true)]    // ambos por debajo
    public void EnRiesgoSegunPromedioYAsistencia(double promedio, int asistencia, bool esperado)
    {
        Assert.AreEqual(esperado, RiesgoAcademico.EnRiesgo(promedio, asistencia));
    }
}
