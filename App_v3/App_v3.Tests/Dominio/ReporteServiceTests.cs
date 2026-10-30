using App_v3.Dominio;

namespace App_v3.Tests;

[TestClass]
public sealed class ReporteServiceTests
{
    private readonly EscenarioDePrueba _e = new();

    [TestMethod]
    public async Task ElReporteDeAsistenciasListaCadaRegistroConSuEstadoDeLoMasRecienteALoMasViejo()
    {
        var comision = await _e.NuevaComisionAsync();
        var ana = await _e.NuevoInscriptoAsync(comision, "Ana", "Gómez");
        await _e.TomarAsistenciaAsync(await _e.NuevaClaseAsync(comision, -14), ana);
        await _e.TomarAsistenciaAsync(await _e.NuevaClaseAsync(comision, -7));

        var filas = await _e.Reportes.AsistenciasAsync(comision.Id);

        Assert.HasCount(2, filas);
        Assert.AreEqual(2, filas[0].Clase.Numero);
        Assert.AreEqual(EstadoAsistencia.Presente, filas[0].Estado);
        Assert.AreEqual(EstadoAsistencia.Ausente, filas[1].Estado);
    }

    [TestMethod]
    public async Task ElReporteDeAsistenciasIncluyeALosEstudiantesDadosDeBaja()
    {
        var comision = await _e.NuevaComisionAsync();
        var beto = await _e.NuevoInscriptoAsync(comision, "Beto", "Pérez");
        await _e.TomarAsistenciaAsync(await _e.NuevaClaseAsync(comision, -7), beto);
        await _e.Inscripciones.DarDeBajaAsync(beto.Id, comision.Id, _e.Hoy);

        Assert.HasCount(1, await _e.Reportes.AsistenciasAsync(comision.Id));
    }

    [TestMethod]
    public async Task ElReporteDeAsistenciasFiltraPorDniAunqueSeEscribaConPuntos()
    {
        var comision = await _e.NuevaComisionAsync();
        var ana = await _e.Estudiantes.AltaAsync(new DatosEstudiante("L1", "30123456", "Ana", "Gómez", "a@b.com"));
        var beto = await _e.Estudiantes.AltaAsync(new DatosEstudiante("L2", "40999888", "Beto", "Pérez", "b@b.com"));
        await _e.Inscripciones.InscribirAsync(ana.Id, comision.Id);
        await _e.Inscripciones.InscribirAsync(beto.Id, comision.Id);
        await _e.TomarAsistenciaAsync(await _e.NuevaClaseAsync(comision, -1));

        var filas = await _e.Reportes.AsistenciasAsync(comision.Id, dni: "30.123");

        Assert.HasCount(1, filas);
        Assert.AreEqual("Ana Gómez", filas[0].Estudiante.NombreCompleto);
    }

    [TestMethod]
    public async Task ElReporteDeAsistenciasFiltraPorRangoDeFechasInclusivo()
    {
        var comision = await _e.NuevaComisionAsync();
        await _e.NuevoInscriptoAsync(comision);
        await _e.TomarAsistenciaAsync(await _e.NuevaClaseAsync(comision, -20));
        await _e.TomarAsistenciaAsync(await _e.NuevaClaseAsync(comision, -10));
        await _e.TomarAsistenciaAsync(await _e.NuevaClaseAsync(comision, -1));

        var filas = await _e.Reportes.AsistenciasAsync(comision.Id, desde: _e.Hoy.AddDays(-10), hasta: _e.Hoy.AddDays(-1));

        Assert.HasCount(2, filas);
    }

    [TestMethod]
    public async Task ElReporteDeAsistenciasNoMezclaComisiones()
    {
        var comisionA = await _e.NuevaComisionAsync("Comisión A");
        var comisionB = await _e.NuevaComisionAsync("Comisión B");
        await _e.NuevoInscriptoAsync(comisionA);
        await _e.NuevoInscriptoAsync(comisionB, "Beto", "Pérez");
        await _e.TomarAsistenciaAsync(await _e.NuevaClaseAsync(comisionA, -1));
        await _e.TomarAsistenciaAsync(await _e.NuevaClaseAsync(comisionB, -1));

        Assert.HasCount(1, await _e.Reportes.AsistenciasAsync(comisionA.Id));
    }

    [TestMethod]
    public async Task ElReporteDeEntregasCruzaCadaEstudianteActivoConCadaTrabajo()
    {
        var comision = await _e.NuevaComisionAsync();
        var ana = await _e.NuevoInscriptoAsync(comision, "Ana", "Gómez");
        await _e.NuevoInscriptoAsync(comision, "Beto", "Pérez");
        var baja = await _e.NuevoInscriptoAsync(comision, "Carla", "Díaz");
        await _e.Inscripciones.DarDeBajaAsync(baja.Id, comision.Id, _e.Hoy);
        var vencido = await _e.NuevoTrabajoAsync(comision, "TP 1", -5);
        await _e.NuevoTrabajoAsync(comision, "TP 2", 5);
        await _e.Entregas.GuardarAsync(vencido.Id, [new LineaEntrega(ana, true)]);

        var filas = await _e.Reportes.EntregasAsync(comision.Id);

        Assert.HasCount(4, filas);
        Assert.AreEqual(EstadoEntrega.Entregado, filas.Single(f => f.Estudiante.Id == ana.Id && f.Trabajo.Nombre == "TP 1").Estado);
        Assert.AreEqual(EstadoEntrega.Pendiente, filas.Single(f => f.Estudiante.Id == ana.Id && f.Trabajo.Nombre == "TP 2").Estado);
        Assert.AreEqual(EstadoEntrega.NoEntregado, filas.Single(f => f.Estudiante.Nombre == "Beto" && f.Trabajo.Nombre == "TP 1").Estado);
    }

    [TestMethod]
    public async Task ElReporteDeEntregasFiltraPorTrabajoYPorDni()
    {
        var comision = await _e.NuevaComisionAsync();
        var ana = await _e.Estudiantes.AltaAsync(new DatosEstudiante("L1", "30123456", "Ana", "Gómez", "a@b.com"));
        var beto = await _e.Estudiantes.AltaAsync(new DatosEstudiante("L2", "40999888", "Beto", "Pérez", "b@b.com"));
        await _e.Inscripciones.InscribirAsync(ana.Id, comision.Id);
        await _e.Inscripciones.InscribirAsync(beto.Id, comision.Id);
        var tp1 = await _e.NuevoTrabajoAsync(comision, "TP 1", -5);
        await _e.NuevoTrabajoAsync(comision, "TP 2", -2);

        var soloTp1 = await _e.Reportes.EntregasAsync(comision.Id, trabajoId: tp1.Id);
        var soloBeto = await _e.Reportes.EntregasAsync(comision.Id, dni: "40.999");

        Assert.HasCount(2, soloTp1);
        Assert.IsTrue(soloTp1.All(f => f.Trabajo.Id == tp1.Id));
        Assert.HasCount(2, soloBeto);
        Assert.IsTrue(soloBeto.All(f => f.Estudiante.Id == beto.Id));
    }
}
