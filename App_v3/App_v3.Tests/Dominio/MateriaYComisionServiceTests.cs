using App_v3.Dominio;

namespace App_v3.Tests;

[TestClass]
public sealed class MateriaYComisionServiceTests
{
    private readonly EscenarioDePrueba _e = new();

    [TestMethod]
    public async Task AltaDeMateriaLaGuardaSinEspacios()
    {
        var materia = await _e.Materias.AltaAsync("  Programación  ");

        Assert.AreEqual("Programación", materia.Nombre);
        Assert.HasCount(1, await _e.Materias.ListarAsync());
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public async Task AltaDeMateriaSinNombreEsRechazada(string? nombre)
    {
        await Assert.ThrowsAsync<ReglaDeNegocioException>(() => _e.Materias.AltaAsync(nombre));

        Assert.IsEmpty(await _e.Materias.ListarAsync());
    }

    [TestMethod]
    public async Task NoSePuedeRepetirUnaMateria()
    {
        await _e.Materias.AltaAsync("Programación");

        await Assert.ThrowsAsync<ReglaDeNegocioException>(() => _e.Materias.AltaAsync("PROGRAMACIÓN"));

        Assert.HasCount(1, await _e.Materias.ListarAsync());
    }

    [TestMethod]
    public async Task LasMateriasSeListanOrdenadasPorNombre()
    {
        await _e.Materias.AltaAsync("Matemática");
        await _e.Materias.AltaAsync("Base de datos");

        var materias = await _e.Materias.ListarAsync();

        Assert.AreEqual("Base de datos", materias[0].Nombre);
        Assert.AreEqual("Matemática", materias[1].Nombre);
    }

    [TestMethod]
    public async Task AltaDeComisionLaAsociaALaMateria()
    {
        var materia = await _e.Materias.AltaAsync("Programación");

        var comision = await _e.Comisiones.AltaAsync(materia.Id, "Comisión A");

        Assert.AreEqual(materia.Id, comision.MateriaId);
        Assert.HasCount(1, await _e.Comisiones.ListarAsync(materia.Id));
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("  ")]
    public async Task AltaDeComisionSinNombreEsRechazada(string? nombre)
    {
        var materia = await _e.Materias.AltaAsync("Programación");

        await Assert.ThrowsAsync<ReglaDeNegocioException>(() => _e.Comisiones.AltaAsync(materia.Id, nombre));
    }

    [TestMethod]
    public async Task NoSeCreaUnaComisionDeUnaMateriaInexistente()
    {
        var ex = await Assert.ThrowsAsync<ReglaDeNegocioException>(() => _e.Comisiones.AltaAsync(999, "Comisión A"));

        Assert.IsTrue(ex.Message.Contains("materia no existe"));
    }

    [TestMethod]
    public async Task ElNombreDeComisionNoSeRepiteDentroDeLaMateria()
    {
        var materia = await _e.Materias.AltaAsync("Programación");
        await _e.Comisiones.AltaAsync(materia.Id, "Comisión A");

        await Assert.ThrowsAsync<ReglaDeNegocioException>(() => _e.Comisiones.AltaAsync(materia.Id, "comisión a"));

        Assert.HasCount(1, await _e.Comisiones.ListarAsync());
    }

    [TestMethod]
    public async Task ElMismoNombreDeComisionPuedeExistirEnOtraMateria()
    {
        var programacion = await _e.Materias.AltaAsync("Programación");
        var matematica = await _e.Materias.AltaAsync("Matemática");

        await _e.Comisiones.AltaAsync(programacion.Id, "Comisión A");
        await _e.Comisiones.AltaAsync(matematica.Id, "Comisión A");

        Assert.HasCount(1, await _e.Comisiones.ListarAsync(programacion.Id));
        Assert.HasCount(2, await _e.Comisiones.ListarAsync());
    }
}
