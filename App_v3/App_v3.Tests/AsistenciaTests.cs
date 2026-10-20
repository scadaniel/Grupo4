using App_v3.Core;

namespace App_v3.Tests;

[TestClass]
public sealed class AsistenciaTests
{
    private static readonly DateOnly Fecha = new(2026, 10, 14);

    private static Nomina NominaConDosActivos()
    {
        var nomina = new Nomina();
        nomina.Alta(new FichaEstudiante("L1", "111", "Juan", "Pérez", "j@mail.com"));
        nomina.Alta(new FichaEstudiante("L2", "222", "María", "Gómez", "m@mail.com"));
        return nomina;
    }

    [TestMethod]
    public void CA3_1_TodosLosActivosQuedanPresentesPorDefecto()
    {
        var servicio = new ServicioAsistencia();

        var lista = servicio.IniciarToma(Fecha, NominaConDosActivos());

        Assert.HasCount(2, lista.Estados);
        Assert.IsTrue(lista.Estados["L1"]);
        Assert.IsTrue(lista.Estados["L2"]);
    }

    [TestMethod]
    public void CA3_1_LosInactivosNoAparecenEnLaGrilla()
    {
        var nomina = NominaConDosActivos();
        nomina.Inactivar("L2");
        var servicio = new ServicioAsistencia();

        var lista = servicio.IniciarToma(Fecha, nomina);

        Assert.HasCount(1, lista.Estados);
        Assert.IsFalse(lista.Estados.ContainsKey("L2"));
    }

    [TestMethod]
    public void CA3_1_SinActivosNoSePuedeIniciarLaToma()
    {
        var servicio = new ServicioAsistencia();

        Assert.Throws<InvalidOperationException>(() => servicio.IniciarToma(Fecha, new Nomina()));
    }

    [TestMethod]
    public void CA3_2_DesmarcarPasaAAusente()
    {
        var lista = new ServicioAsistencia().IniciarToma(Fecha, NominaConDosActivos());

        lista.MarcarAusente("L1");

        Assert.IsFalse(lista.Estados["L1"]);
        Assert.IsTrue(lista.Estados["L2"]); // el otro no cambia
    }

    [TestMethod]
    public void CA3_2_SePuedeVolverAMarcarPresente()
    {
        var lista = new ServicioAsistencia().IniciarToma(Fecha, NominaConDosActivos());
        lista.MarcarAusente("L1");

        lista.MarcarPresente("L1");

        Assert.IsTrue(lista.Estados["L1"]);
    }

    [TestMethod]
    public void CA3_2_MarcarLegajoQueNoEstaEnLaListaLanzaExcepcion()
    {
        var lista = new ServicioAsistencia().IniciarToma(Fecha, NominaConDosActivos());

        Assert.Throws<KeyNotFoundException>(() => lista.MarcarAusente("L99"));
    }

    [TestMethod]
    public void CA3_3_GuardarAlmacenaTodosLosEstadosDeLaFecha()
    {
        var servicio = new ServicioAsistencia();
        var lista = servicio.IniciarToma(Fecha, NominaConDosActivos());
        lista.MarcarAusente("L2");

        servicio.Guardar(lista);

        var guardado = servicio.Obtener(Fecha);
        Assert.IsNotNull(guardado);
        Assert.HasCount(2, guardado);
        Assert.IsTrue(guardado["L1"]);
        Assert.IsFalse(guardado["L2"]);
    }

    [TestMethod]
    public void CA3_3_LoGuardadoNoCambiaSiSeSigueEditandoLaLista()
    {
        var servicio = new ServicioAsistencia();
        var lista = servicio.IniciarToma(Fecha, NominaConDosActivos());
        servicio.Guardar(lista);

        lista.MarcarAusente("L1");

        Assert.IsTrue(servicio.Obtener(Fecha)!["L1"]);
    }

    [TestMethod]
    public void CA3_4_FechaSinRegistroNoExiste()
    {
        var servicio = new ServicioAsistencia();

        Assert.IsFalse(servicio.ExisteRegistro(Fecha));
        Assert.IsNull(servicio.Obtener(Fecha));
    }

    [TestMethod]
    public void CA3_4_IniciarTomaEnFechaYaGuardadaNotifica()
    {
        var servicio = new ServicioAsistencia();
        var nomina = NominaConDosActivos();
        servicio.Guardar(servicio.IniciarToma(Fecha, nomina));

        Assert.IsTrue(servicio.ExisteRegistro(Fecha));
        Assert.Throws<AsistenciaYaRegistradaException>(() => servicio.IniciarToma(Fecha, nomina));
    }

    [TestMethod]
    public void CA3_4_GuardarDosVecesLaMismaFechaLanzaExcepcion()
    {
        var servicio = new ServicioAsistencia();
        var lista = servicio.IniciarToma(Fecha, NominaConDosActivos());
        servicio.Guardar(lista);

        Assert.Throws<AsistenciaYaRegistradaException>(() => servicio.Guardar(lista));
    }
}