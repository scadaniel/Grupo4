using App_v3.Dominio;

namespace App_v3.Tests;

[TestClass]
public sealed class EstudianteServiceTests
{
    private readonly EscenarioDePrueba _e = new();

    private static DatosEstudiante Datos(
        string? legajo = "L1", string? dni = "30111222", string? nombre = "Juan",
        string? apellido = "Pérez", string? email = "juan@mail.com", string? telefono = null) =>
        new(legajo, dni, nombre, apellido, email, telefono);

    [TestMethod]
    public async Task CA1_1_AltaGuardaAlEstudianteYLeAsignaId()
    {
        var estudiante = await _e.Estudiantes.AltaAsync(Datos());

        Assert.AreNotEqual(0, estudiante.Id);
        Assert.HasCount(1, await _e.Estudiantes.ListarAsync());
    }

    [TestMethod]
    public async Task CA1_1_AltaQuitaEspaciosSobrantes()
    {
        var estudiante = await _e.Estudiantes.AltaAsync(Datos(legajo: " L9 ", nombre: " Ana ", email: " ana@mail.com "));

        Assert.AreEqual("L9", estudiante.Legajo);
        Assert.AreEqual("Ana", estudiante.Nombre);
        Assert.AreEqual("ana@mail.com", estudiante.Email);
    }

    [TestMethod]
    public async Task CA1_1_TelefonoEsOpcional()
    {
        var sin = await _e.Estudiantes.AltaAsync(Datos(legajo: "L1", dni: "30111222", telefono: null));
        var con = await _e.Estudiantes.AltaAsync(Datos(legajo: "L2", dni: "30111223", telefono: " 1155550000 "));

        Assert.AreEqual(string.Empty, sin.Telefono);
        Assert.AreEqual("1155550000", con.Telefono);
    }

    [TestMethod]
    [DataRow(null, "30111222", "Juan", "Pérez", "juan@mail.com", "Legajo")]
    [DataRow("L1", "  ", "Juan", "Pérez", "juan@mail.com", "DNI")]
    [DataRow("L1", "30111222", "", "Pérez", "juan@mail.com", "Nombre")]
    [DataRow("L1", "30111222", "Juan", null, "juan@mail.com", "Apellido")]
    [DataRow("L1", "30111222", "Juan", "Pérez", "", "Email")]
    public async Task CA1_1_AltaSinCampoObligatorioNoGuardaYLoNombra(
        string? legajo, string? dni, string? nombre, string? apellido, string? email, string campo)
    {
        var datos = new DatosEstudiante(legajo, dni, nombre, apellido, email);

        var ex = await Assert.ThrowsAsync<ReglaDeNegocioException>(() => _e.Estudiantes.AltaAsync(datos));

        Assert.IsTrue(ex.Message.Contains(campo));
        Assert.IsEmpty(await _e.Estudiantes.ListarAsync());
    }

    [TestMethod]
    public async Task CA1_1_InformaTodosLosCamposFaltantesJuntos()
    {
        var ex = await Assert.ThrowsAsync<ReglaDeNegocioException>(
            () => _e.Estudiantes.AltaAsync(new DatosEstudiante(null, null, "Juan", "Pérez", null)));

        Assert.IsTrue(ex.Message.Contains("Legajo"));
        Assert.IsTrue(ex.Message.Contains("DNI"));
        Assert.IsTrue(ex.Message.Contains("Email"));
    }

    [TestMethod]
    [DataRow("abc")]
    [DataRow("3011122a")]
    [DataRow("123")]
    [DataRow("1234567890")]
    [DataRow("30-111-222")]
    public async Task CA1_1_DniQueNoEsNumericoDeSieteOOchoDigitosEsRechazado(string dni)
    {
        var ex = await Assert.ThrowsAsync<ReglaDeNegocioException>(() => _e.Estudiantes.AltaAsync(Datos(dni: dni)));

        Assert.IsTrue(ex.Message.Contains("DNI"));
        Assert.IsEmpty(await _e.Estudiantes.ListarAsync());
    }

    [TestMethod]
    [DataRow("30111222", "30111222")]
    [DataRow("30.111.222", "30111222")]
    [DataRow(" 30 111 222 ", "30111222")]
    [DataRow("9111222", "9111222")]
    public async Task CA1_1_DniValidoSeGuardaSinPuntosNiEspacios(string escrito, string guardado)
    {
        var estudiante = await _e.Estudiantes.AltaAsync(Datos(dni: escrito));

        Assert.AreEqual(guardado, estudiante.Dni);
    }

    [TestMethod]
    [DataRow("juan")]
    [DataRow("juan@")]
    [DataRow("@mail.com")]
    [DataRow("juan@mail")]
    [DataRow("ju an@mail.com")]
    [DataRow("juan@@mail.com")]
    public async Task CA1_1_EmailInvalidoEsRechazado(string email)
    {
        var ex = await Assert.ThrowsAsync<ReglaDeNegocioException>(() => _e.Estudiantes.AltaAsync(Datos(email: email)));

        Assert.IsTrue(ex.Message.Contains("email"));
        Assert.IsEmpty(await _e.Estudiantes.ListarAsync());
    }

    [TestMethod]
    public async Task CA1_2_LegajoRepetidoEsRechazadoAunqueCambieLaCapitalizacion()
    {
        await _e.Estudiantes.AltaAsync(Datos(legajo: "ab-1", dni: "30111222"));

        var ex = await Assert.ThrowsAsync<ReglaDeNegocioException>(
            () => _e.Estudiantes.AltaAsync(Datos(legajo: "AB-1", dni: "30111223")));

        Assert.IsTrue(ex.Message.Contains("legajo"));
    }

    [TestMethod]
    public async Task CA1_2_DniRepetidoEsRechazadoAunqueSeEscribaConPuntos()
    {
        await _e.Estudiantes.AltaAsync(Datos(legajo: "L1", dni: "30111222"));

        var ex = await Assert.ThrowsAsync<ReglaDeNegocioException>(
            () => _e.Estudiantes.AltaAsync(Datos(legajo: "L2", dni: "30.111.222")));

        Assert.IsTrue(ex.Message.Contains("DNI"));
    }

    [TestMethod]
    public async Task CA1_2_UnDuplicadoNoSeGuarda()
    {
        await _e.Estudiantes.AltaAsync(Datos(legajo: "L1", dni: "30111222"));

        await Assert.ThrowsAsync<ReglaDeNegocioException>(
            () => _e.Estudiantes.AltaAsync(Datos(legajo: "L1", dni: "30111223")));

        Assert.HasCount(1, await _e.Estudiantes.ListarAsync());
    }

    [TestMethod]
    public async Task CA1_2_ElDuplicadoSeControlaContraTodosLosEstudiantesDelSistema()
    {
        var estudiante = await _e.NuevoEstudianteAsync("L1", "30111222");
        var comision = await _e.NuevaComisionAsync();
        await _e.Inscripciones.InscribirAsync(estudiante.Id, comision.Id);
        await _e.Inscripciones.DarDeBajaAsync(estudiante.Id, comision.Id, new DateTime(2026, 9, 1));

        await Assert.ThrowsAsync<ReglaDeNegocioException>(
            () => _e.Estudiantes.AltaAsync(Datos(legajo: "L1", dni: "30999888")));
    }

    [TestMethod]
    public async Task CA1_3_ListarDevuelveTodosOrdenadosPorApellidoYNombre()
    {
        await _e.NuevoEstudianteAsync("L1", "30111222", "Juan", "Pérez");
        await _e.NuevoEstudianteAsync("L2", "30111223", "Ana", "Gómez");
        await _e.NuevoEstudianteAsync("L3", "30111224", "Beto", "Gómez");

        var lista = await _e.Estudiantes.ListarAsync();

        Assert.HasCount(3, lista);
        Assert.AreEqual("Ana Gómez", lista[0].NombreCompleto);
        Assert.AreEqual("Beto Gómez", lista[1].NombreCompleto);
        Assert.AreEqual("Juan Pérez", lista[2].NombreCompleto);
    }

    [TestMethod]
    public async Task ObtenerDevuelveNuloSiElEstudianteNoExiste()
    {
        var creado = await _e.NuevoEstudianteAsync();

        Assert.AreEqual(creado.Dni, (await _e.Estudiantes.ObtenerAsync(creado.Id))!.Dni);
        Assert.IsNull(await _e.Estudiantes.ObtenerAsync(999));
    }
}
