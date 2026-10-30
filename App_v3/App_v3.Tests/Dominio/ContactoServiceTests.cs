using App_v3.Dominio;

namespace App_v3.Tests;

[TestClass]
public sealed class ContactoServiceTests
{
    private readonly EscenarioDePrueba _e = new();

    [TestMethod]
    public async Task CA10_1_UnContactoValidoQuedaEnElHistorialDelEstudiante()
    {
        var comision = await _e.NuevaComisionAsync();
        var ana = await _e.NuevoInscriptoAsync(comision);

        await _e.Contactos.RegistrarAsync(new DatosContacto(
            ana.Id, comision.Id, new DateTime(2026, 10, 28, 15, 0, 0), MedioContacto.Email, "  Respondió ", " Se acordó entregar el TP "));

        var historial = await _e.Contactos.ListarAsync(ana.Id, comision.Id);
        Assert.HasCount(1, historial);
        Assert.AreEqual(new DateTime(2026, 10, 28), historial[0].Fecha);
        Assert.AreEqual(MedioContacto.Email, historial[0].Medio);
        Assert.AreEqual("Respondió", historial[0].Resultado);
        Assert.AreEqual("Se acordó entregar el TP", historial[0].Nota);
    }

    [TestMethod]
    [DataRow(false, "Respondió", "Habló con él", "Fecha")]
    [DataRow(true, "", "Habló con él", "Resultado")]
    [DataRow(true, "Respondió", "   ", "Nota")]
    [DataRow(false, null, null, "Fecha, Resultado, Nota")]
    public async Task CA10_2_AvisaTodosLosCamposQueFaltanYNoGuardaNada(
        bool conFecha, string? resultado, string? nota, string faltantes)
    {
        var comision = await _e.NuevaComisionAsync();
        var ana = await _e.NuevoInscriptoAsync(comision);
        DateTime? fecha = conFecha ? _e.Hoy : null;

        var ex = await Assert.ThrowsAsync<ReglaDeNegocioException>(() =>
            _e.Contactos.RegistrarAsync(new DatosContacto(ana.Id, comision.Id, fecha, MedioContacto.Llamada, resultado, nota)));

        Assert.AreEqual($"Faltan completar: {faltantes}.", ex.Message);
        Assert.IsEmpty(await _e.Contactos.ListarAsync(ana.Id, comision.Id));
    }

    [TestMethod]
    public async Task NoSeRegistraUnContactoConAlguienQueNoEstaInscriptoEnLaComision()
    {
        var comision = await _e.NuevaComisionAsync();
        var otro = await _e.NuevoEstudianteAsync();

        var ex = await Assert.ThrowsAsync<ReglaDeNegocioException>(() =>
            _e.Contactos.RegistrarAsync(new DatosContacto(otro.Id, comision.Id, _e.Hoy, MedioContacto.Llamada, "Respondió", "ok")));

        Assert.IsTrue(ex.Message.Contains("no está inscripto"));
    }

    [TestMethod]
    public async Task ElHistorialMuestraPrimeroElContactoMasReciente()
    {
        var comision = await _e.NuevaComisionAsync();
        var ana = await _e.NuevoInscriptoAsync(comision);
        await _e.Contactos.RegistrarAsync(new DatosContacto(ana.Id, comision.Id, _e.Hoy.AddDays(-4), MedioContacto.Llamada, "Respondió", "viejo"));
        await _e.Contactos.RegistrarAsync(new DatosContacto(ana.Id, comision.Id, _e.Hoy.AddDays(-1), MedioContacto.Email, "Respondió", "nuevo"));

        var historial = await _e.Contactos.ListarAsync(ana.Id, comision.Id);

        Assert.AreEqual("nuevo", historial[0].Nota);
        Assert.AreEqual("viejo", historial[1].Nota);
    }

    [TestMethod]
    public async Task ElHistorialDeUnaComisionNoTraeLosContactosDeOtra()
    {
        var comisionA = await _e.NuevaComisionAsync("Comisión A");
        var comisionB = await _e.NuevaComisionAsync("Comisión B");
        var ana = await _e.NuevoEstudianteAsync();
        await _e.Inscripciones.InscribirAsync(ana.Id, comisionA.Id);
        await _e.Inscripciones.InscribirAsync(ana.Id, comisionB.Id);
        await _e.Contactos.RegistrarAsync(new DatosContacto(ana.Id, comisionA.Id, _e.Hoy, MedioContacto.Llamada, "Respondió", "ok"));

        Assert.HasCount(1, await _e.Contactos.ListarAsync(ana.Id, comisionA.Id));
        Assert.IsEmpty(await _e.Contactos.ListarAsync(ana.Id, comisionB.Id));
    }
}
