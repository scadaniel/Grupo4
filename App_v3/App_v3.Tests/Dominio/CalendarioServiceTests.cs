using App_v3.Dominio;

namespace App_v3.Tests;

[TestClass]
public sealed class CalendarioServiceTests
{
    private readonly EscenarioDePrueba _e = new();

    [TestMethod]
    public async Task CA2_1_AgregarUnaClaseLaDejaRegistradaConSuFechaYSuTema()
    {
        var comision = await _e.NuevaComisionAsync();

        var clase = await _e.Calendario.AgregarClaseAsync(comision.Id, new DateTime(2026, 11, 3, 18, 30, 0), "Variables");

        var clases = await _e.Calendario.ListarClasesAsync(comision.Id);
        Assert.HasCount(1, clases);
        Assert.AreEqual(clase.Id, clases[0].Id);
        Assert.AreEqual(new DateTime(2026, 11, 3), clases[0].Fecha);
        Assert.AreEqual("Variables", clases[0].Tema);
        Assert.AreEqual(1, clases[0].Numero);
    }

    [TestMethod]
    public async Task CA2_1_LasClasesSeNumeranEnOrdenDeCargaDentroDeCadaComision()
    {
        var comisionA = await _e.NuevaComisionAsync("Comisión A");
        var comisionB = await _e.NuevaComisionAsync("Comisión B");

        await _e.NuevaClaseAsync(comisionA, 1);
        var segundaDeA = await _e.NuevaClaseAsync(comisionA, 8);
        var primeraDeB = await _e.NuevaClaseAsync(comisionB, 1);

        Assert.AreEqual(2, segundaDeA.Numero);
        Assert.AreEqual(1, primeraDeB.Numero);
    }

    [TestMethod]
    public async Task CA2_1_ElTemaSeGuardaSinEspaciosSobrantes()
    {
        var comision = await _e.NuevaComisionAsync();

        var clase = await _e.Calendario.AgregarClaseAsync(comision.Id, _e.Hoy, "  Ciclos  ");

        Assert.AreEqual("Ciclos", clase.Tema);
    }

    [TestMethod]
    public async Task VariasComisionesPuedenTenerClaseElMismoDia()
    {
        var comisionA = await _e.NuevaComisionAsync("Comisión A");
        var comisionB = await _e.NuevaComisionAsync("Comisión B");

        await _e.NuevaClaseAsync(comisionA, 1);
        await _e.NuevaClaseAsync(comisionB, 1);

        Assert.HasCount(1, await _e.Calendario.ListarClasesAsync(comisionA.Id));
        Assert.HasCount(1, await _e.Calendario.ListarClasesAsync(comisionB.Id));
    }

    [TestMethod]
    public async Task NoSeCargaDosVecesUnaClaseElMismoDiaEnLaMismaComision()
    {
        var comision = await _e.NuevaComisionAsync();
        await _e.NuevaClaseAsync(comision, 1);

        var ex = await Assert.ThrowsAsync<ReglaDeNegocioException>(() => _e.NuevaClaseAsync(comision, 1, "Otra"));

        Assert.IsTrue(ex.Message.Contains("Ya hay una clase"));
        Assert.HasCount(1, await _e.Calendario.ListarClasesAsync(comision.Id));
    }

    [TestMethod]
    public async Task CA2_2_AgregarUnTrabajoLoAnadeConSuNombreYSuFechaLimite()
    {
        var comision = await _e.NuevaComisionAsync();

        await _e.Calendario.AgregarTrabajoAsync(comision.Id, "TP 1", new DateTime(2026, 11, 20));

        var trabajos = await _e.Calendario.ListarTrabajosAsync(comision.Id);
        Assert.HasCount(1, trabajos);
        Assert.AreEqual("TP 1", trabajos[0].Nombre);
        Assert.AreEqual(new DateTime(2026, 11, 20), trabajos[0].FechaLimite);
    }

    [TestMethod]
    public async Task ElNombreDeUnTrabajoNoSeRepiteEnLaComisionPeroSiEnOtra()
    {
        var comisionA = await _e.NuevaComisionAsync("Comisión A");
        var comisionB = await _e.NuevaComisionAsync("Comisión B");
        await _e.NuevoTrabajoAsync(comisionA, "TP 1", 7);

        await Assert.ThrowsAsync<ReglaDeNegocioException>(() => _e.NuevoTrabajoAsync(comisionA, "tp 1", 9));
        await _e.NuevoTrabajoAsync(comisionB, "TP 1", 7);

        Assert.HasCount(1, await _e.Calendario.ListarTrabajosAsync(comisionA.Id));
        Assert.HasCount(1, await _e.Calendario.ListarTrabajosAsync(comisionB.Id));
    }

    [TestMethod]
    public async Task CA2_3_ClasesYTrabajosSeListanJuntosEnOrdenCronologico()
    {
        var comision = await _e.NuevaComisionAsync();
        await _e.NuevoTrabajoAsync(comision, "TP 1", 10);
        await _e.NuevaClaseAsync(comision, 14, "Ciclos");
        await _e.NuevaClaseAsync(comision, 3, "Variables");

        var eventos = await _e.Calendario.ListarEventosAsync(comision.Id);

        Assert.HasCount(3, eventos);
        Assert.AreEqual("Clase 2: Variables", eventos[0].Descripcion);
        Assert.AreEqual(TipoEvento.TrabajoPractico, eventos[1].Tipo);
        Assert.AreEqual("Entrega: TP 1", eventos[1].Descripcion);
        Assert.AreEqual("Clase 1: Ciclos", eventos[2].Descripcion);
    }

    [TestMethod]
    public async Task CA2_3_ElCalendarioSoloMuestraLoDeLaComisionElegida()
    {
        var comisionA = await _e.NuevaComisionAsync("Comisión A");
        var comisionB = await _e.NuevaComisionAsync("Comisión B");
        await _e.NuevaClaseAsync(comisionA, 1);
        await _e.NuevoTrabajoAsync(comisionB, "TP B", 5);

        Assert.HasCount(1, await _e.Calendario.ListarEventosAsync(comisionA.Id));
        Assert.HasCount(1, await _e.Calendario.ListarEventosAsync(comisionB.Id));
    }

    [TestMethod]
    [DataRow("2026-11-03", "", "Tema")]
    [DataRow("2026-11-03", null, "Tema")]
    [DataRow(null, "Variables", "Fecha")]
    [DataRow(null, null, "Fecha, Tema")]
    public async Task CA2_4_UnaClaseSinLosDatosObligatoriosNoSeGuardaYSePideCompletarlos(
        string? fecha, string? tema, string faltantes)
    {
        var comision = await _e.NuevaComisionAsync();
        DateTime? dia = fecha is null ? null : DateTime.Parse(fecha);

        var ex = await Assert.ThrowsAsync<ReglaDeNegocioException>(
            () => _e.Calendario.AgregarClaseAsync(comision.Id, dia, tema));

        Assert.IsTrue(ex.Message.Contains("Complete los campos obligatorios"));
        Assert.IsTrue(ex.Message.Contains(faltantes));
        Assert.IsEmpty(await _e.Calendario.ListarClasesAsync(comision.Id));
    }

    [TestMethod]
    [DataRow("", "2026-11-20", "Nombre")]
    [DataRow("TP 1", null, "Fecha límite")]
    [DataRow(null, null, "Nombre, Fecha límite")]
    public async Task CA2_4_UnTrabajoSinLosDatosObligatoriosNoSeGuardaYSePideCompletarlos(
        string? nombre, string? fecha, string faltantes)
    {
        var comision = await _e.NuevaComisionAsync();
        DateTime? dia = fecha is null ? null : DateTime.Parse(fecha);

        var ex = await Assert.ThrowsAsync<ReglaDeNegocioException>(
            () => _e.Calendario.AgregarTrabajoAsync(comision.Id, nombre, dia));

        Assert.IsTrue(ex.Message.Contains(faltantes));
        Assert.IsEmpty(await _e.Calendario.ListarTrabajosAsync(comision.Id));
    }

    [TestMethod]
    public async Task NoSeCargaCalendarioEnUnaComisionQueNoExiste()
    {
        var ex = await Assert.ThrowsAsync<ReglaDeNegocioException>(
            () => _e.Calendario.AgregarClaseAsync(999, _e.Hoy, "Tema"));

        Assert.IsTrue(ex.Message.Contains("comisión"));
    }
}
