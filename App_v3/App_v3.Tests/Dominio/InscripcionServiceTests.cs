using App_v3.Dominio;

namespace App_v3.Tests;

[TestClass]
public sealed class InscripcionServiceTests
{
    private readonly EscenarioDePrueba _e = new();
    private static readonly DateTime Baja = new(2026, 9, 15);

    [TestMethod]
    public async Task InscribirCreaUnaInscripcionActiva()
    {
        var estudiante = await _e.NuevoEstudianteAsync();
        var comision = await _e.NuevaComisionAsync();

        var inscripcion = await _e.Inscripciones.InscribirAsync(estudiante.Id, comision.Id);

        Assert.IsTrue(inscripcion.Activa);
        Assert.IsNull(inscripcion.FechaBaja);
        Assert.HasCount(1, await _e.Inscripciones.ListarActivosAsync(comision.Id));
    }

    [TestMethod]
    public async Task UnEstudiantePuedeCursarVariasComisiones()
    {
        var estudiante = await _e.NuevoEstudianteAsync();
        var comisionA = await _e.NuevaComisionAsync("Comisión A");
        var comisionB = await _e.NuevaComisionAsync("Comisión B");

        await _e.Inscripciones.InscribirAsync(estudiante.Id, comisionA.Id);
        await _e.Inscripciones.InscribirAsync(estudiante.Id, comisionB.Id);

        Assert.HasCount(1, await _e.Inscripciones.ListarActivosAsync(comisionA.Id));
        Assert.HasCount(1, await _e.Inscripciones.ListarActivosAsync(comisionB.Id));
    }

    [TestMethod]
    public async Task NoSePuedeInscribirDosVecesALaMismaComision()
    {
        var estudiante = await _e.NuevoEstudianteAsync();
        var comision = await _e.NuevaComisionAsync();
        await _e.Inscripciones.InscribirAsync(estudiante.Id, comision.Id);

        var ex = await Assert.ThrowsAsync<ReglaDeNegocioException>(
            () => _e.Inscripciones.InscribirAsync(estudiante.Id, comision.Id));

        Assert.IsTrue(ex.Message.Contains("ya está inscripto"));
        Assert.HasCount(1, await _e.Inscripciones.ListarNominaAsync(comision.Id));
    }

    [TestMethod]
    public async Task NoSeInscribeUnEstudianteQueNoExiste()
    {
        var comision = await _e.NuevaComisionAsync();

        var ex = await Assert.ThrowsAsync<ReglaDeNegocioException>(
            () => _e.Inscripciones.InscribirAsync(999, comision.Id));

        Assert.IsTrue(ex.Message.Contains("estudiante no existe"));
    }

    [TestMethod]
    public async Task NoSeInscribeEnUnaComisionQueNoExiste()
    {
        var estudiante = await _e.NuevoEstudianteAsync();

        var ex = await Assert.ThrowsAsync<ReglaDeNegocioException>(
            () => _e.Inscripciones.InscribirAsync(estudiante.Id, 999));

        Assert.IsTrue(ex.Message.Contains("comisión no existe"));
    }

    [TestMethod]
    public async Task CA1_4_LaBajaNoBorraNadaYRegistraLaFecha()
    {
        var estudiante = await _e.NuevoEstudianteAsync();
        var comision = await _e.NuevaComisionAsync();
        await _e.Inscripciones.InscribirAsync(estudiante.Id, comision.Id);

        await _e.Inscripciones.DarDeBajaAsync(estudiante.Id, comision.Id, Baja);

        var nomina = await _e.Inscripciones.ListarNominaAsync(comision.Id);
        Assert.HasCount(1, nomina);
        Assert.IsFalse(nomina[0].Activo);
        Assert.AreEqual(Baja, nomina[0].FechaBaja);
        Assert.IsNotNull(await _e.Estudiantes.ObtenerAsync(estudiante.Id));
    }

    [TestMethod]
    public async Task CA1_4_LaBajaEnUnaComisionNoAfectaALaOtra()
    {
        var estudiante = await _e.NuevoEstudianteAsync();
        var comisionA = await _e.NuevaComisionAsync("Comisión A");
        var comisionB = await _e.NuevaComisionAsync("Comisión B");
        await _e.Inscripciones.InscribirAsync(estudiante.Id, comisionA.Id);
        await _e.Inscripciones.InscribirAsync(estudiante.Id, comisionB.Id);

        await _e.Inscripciones.DarDeBajaAsync(estudiante.Id, comisionA.Id, Baja);

        Assert.IsEmpty(await _e.Inscripciones.ListarActivosAsync(comisionA.Id));
        Assert.HasCount(1, await _e.Inscripciones.ListarActivosAsync(comisionB.Id));
    }

    [TestMethod]
    public async Task DarDeBajaDosVecesConservaLaPrimeraFecha()
    {
        var estudiante = await _e.NuevoEstudianteAsync();
        var comision = await _e.NuevaComisionAsync();
        await _e.Inscripciones.InscribirAsync(estudiante.Id, comision.Id);
        await _e.Inscripciones.DarDeBajaAsync(estudiante.Id, comision.Id, Baja);

        await _e.Inscripciones.DarDeBajaAsync(estudiante.Id, comision.Id, Baja.AddDays(10));

        var nomina = await _e.Inscripciones.ListarNominaAsync(comision.Id);
        Assert.AreEqual(Baja, nomina[0].FechaBaja);
    }

    [TestMethod]
    public async Task NoSePuedeDarDeBajaAQuienNoEstaInscripto()
    {
        var estudiante = await _e.NuevoEstudianteAsync();
        var comision = await _e.NuevaComisionAsync();

        await Assert.ThrowsAsync<ReglaDeNegocioException>(
            () => _e.Inscripciones.DarDeBajaAsync(estudiante.Id, comision.Id, Baja));
    }

    [TestMethod]
    public async Task CA6_4_LosInactivosNoEstanEntreLosActivos()
    {
        var comision = await _e.NuevaComisionAsync();
        var activo = await _e.NuevoEstudianteAsync("L1", "30111222", "Ana", "Gómez");
        var inactivo = await _e.NuevoEstudianteAsync("L2", "30111223", "Beto", "Pérez");
        await _e.Inscripciones.InscribirAsync(activo.Id, comision.Id);
        await _e.Inscripciones.InscribirAsync(inactivo.Id, comision.Id);
        await _e.Inscripciones.DarDeBajaAsync(inactivo.Id, comision.Id, Baja);

        var activos = await _e.Inscripciones.ListarActivosAsync(comision.Id);

        Assert.HasCount(1, activos);
        Assert.AreEqual("Ana Gómez", activos[0].NombreCompleto);
    }

    [TestMethod]
    public async Task CA13_1_ListarInactivosMuestraSoloLosDadosDeBaja()
    {
        var comision = await _e.NuevaComisionAsync();
        var activo = await _e.NuevoEstudianteAsync("L1", "30111222", "Ana", "Gómez");
        var inactivo = await _e.NuevoEstudianteAsync("L2", "30111223", "Beto", "Pérez");
        await _e.Inscripciones.InscribirAsync(activo.Id, comision.Id);
        await _e.Inscripciones.InscribirAsync(inactivo.Id, comision.Id);
        await _e.Inscripciones.DarDeBajaAsync(inactivo.Id, comision.Id, Baja);

        var inactivos = await _e.Inscripciones.ListarInactivosAsync(comision.Id);

        Assert.HasCount(1, inactivos);
        Assert.AreEqual("Beto Pérez", inactivos[0].NombreCompleto);
    }

    [TestMethod]
    public async Task ReactivarVuelveAHacerActivoYBorraLaFechaDeBaja()
    {
        var estudiante = await _e.NuevoEstudianteAsync();
        var comision = await _e.NuevaComisionAsync();
        await _e.Inscripciones.InscribirAsync(estudiante.Id, comision.Id);
        await _e.Inscripciones.DarDeBajaAsync(estudiante.Id, comision.Id, Baja);

        await _e.Inscripciones.ReactivarAsync(estudiante.Id, comision.Id);

        var nomina = await _e.Inscripciones.ListarNominaAsync(comision.Id);
        Assert.IsTrue(nomina[0].Activo);
        Assert.IsNull(nomina[0].FechaBaja);
    }

    [TestMethod]
    public async Task InscribirAUnEstudianteDadoDeBajaLoReactivaSinDuplicarLaInscripcion()
    {
        var estudiante = await _e.NuevoEstudianteAsync();
        var comision = await _e.NuevaComisionAsync();
        await _e.Inscripciones.InscribirAsync(estudiante.Id, comision.Id);
        await _e.Inscripciones.DarDeBajaAsync(estudiante.Id, comision.Id, Baja);

        await _e.Inscripciones.InscribirAsync(estudiante.Id, comision.Id);

        var nomina = await _e.Inscripciones.ListarNominaAsync(comision.Id);
        Assert.HasCount(1, nomina);
        Assert.IsTrue(nomina[0].Activo);
    }

    [TestMethod]
    public async Task CA1_3_LaNominaSoloTieneALosInscriptosDeEsaComisionOrdenados()
    {
        var comisionA = await _e.NuevaComisionAsync("Comisión A");
        var comisionB = await _e.NuevaComisionAsync("Comisión B");
        var juan = await _e.NuevoEstudianteAsync("L1", "30111222", "Juan", "Pérez");
        var ana = await _e.NuevoEstudianteAsync("L2", "30111223", "Ana", "Gómez");
        var luis = await _e.NuevoEstudianteAsync("L3", "30111224", "Luis", "Díaz");
        await _e.Inscripciones.InscribirAsync(juan.Id, comisionA.Id);
        await _e.Inscripciones.InscribirAsync(ana.Id, comisionA.Id);
        await _e.Inscripciones.InscribirAsync(luis.Id, comisionB.Id);

        var nomina = await _e.Inscripciones.ListarNominaAsync(comisionA.Id);

        Assert.HasCount(2, nomina);
        Assert.AreEqual("Ana Gómez", nomina[0].Estudiante.NombreCompleto);
        Assert.AreEqual("Juan Pérez", nomina[1].Estudiante.NombreCompleto);
    }
}
