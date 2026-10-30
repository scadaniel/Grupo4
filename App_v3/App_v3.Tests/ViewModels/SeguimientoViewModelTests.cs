using App_v3.Dominio;
using App_v3.ViewModels;

namespace App_v3.Tests;

[TestClass]
public sealed class SeguimientoViewModelTests
{
    private readonly EscenarioDeVistas _v = new();

    private static Task Ejecutar(System.Windows.Input.ICommand c) => SelectorYMateriasViewModelTests.Ejecutar(c);

    /// <summary>Tres estudiantes: Cruz con riesgo alto, Bravo con riesgo medio y Acosta sin riesgo.</summary>
    private async Task<Comision> ComisionConTresNivelesAsync()
    {
        var comision = await _v.ConComisionAsync(("Ana", "Acosta"), ("Beto", "Bravo"), ("Carla", "Cruz"));
        var activos = await _v.Dominio.Inscripciones.ListarActivosAsync(comision.Id);
        var bravo = activos.Single(e => e.Apellido == "Bravo");
        var cruz = activos.Single(e => e.Apellido == "Cruz");
        for (var i = 1; i <= 5; i++)
            await _v.Dominio.TomarAsistenciaAsync(await _v.Dominio.NuevaClaseAsync(comision, -i), cruz, i <= 3 ? bravo : cruz);
        return comision;
    }

    [TestMethod]
    public async Task CA8_1_ElPanelMuestraDeAltoAMedioABajo()
    {
        await ComisionConTresNivelesAsync();
        var p = _v.Seguimiento();

        await p.CargarAsync();

        CollectionEquals(p.Filas.Select(f => f.TextoNivel), "ALTO", "MEDIO", "BAJO");
        CollectionEquals(p.Filas.Select(f => f.NombreCompleto), "Cruz, Carla", "Bravo, Beto", "Acosta, Ana");
    }

    private static void CollectionEquals(IEnumerable<string> actual, params string[] esperado) =>
        Assert.AreEqual(string.Join("|", esperado), string.Join("|", actual));

    [TestMethod]
    public async Task CA8_2_CadaFilaMuestraLosDatosQueJustificanElNivel()
    {
        await ComisionConTresNivelesAsync();
        var p = _v.Seguimiento();
        await p.CargarAsync();

        var cruz = p.Filas[0];

        Assert.AreEqual("Inasistencias: 5", cruz.Inasistencias);
        Assert.AreEqual("TPs vencidos sin entregar: 0", cruz.TpsVencidos);
        Assert.AreEqual("5 inasistencias", cruz.Motivo);
        Assert.AreEqual(NivelRiesgo.Alto, cruz.Nivel);
    }

    [TestMethod]
    public async Task CA8_4_AlCorregirUnaAsistenciaElPanelMuestraElNivelNuevo()
    {
        var comision = await ComisionConTresNivelesAsync();
        var p = _v.Seguimiento();
        await p.CargarAsync();
        Assert.AreEqual("Cruz, Carla", p.Filas[0].NombreCompleto);

        var cruz = (await _v.Dominio.Inscripciones.ListarActivosAsync(comision.Id)).Single(e => e.Apellido == "Cruz");
        foreach (var clase in await _v.Dominio.Calendario.ListarClasesAsync(comision.Id))
            await _v.Dominio.Asistencias.CorregirAsync(clase.Id, [new LineaAsistencia(cruz, Presente: true)]);
        await p.CargarAsync();

        Assert.AreEqual("Bravo, Beto", p.Filas[0].NombreCompleto);
        Assert.AreEqual("BAJO", p.Filas.Single(f => f.Estudiante.Apellido == "Cruz").TextoNivel);
    }

    [TestMethod]
    public async Task CA6_4_UnEstudianteDadoDeBajaDejaDeAparecerEnElPanel()
    {
        var comision = await ComisionConTresNivelesAsync();
        var cruz = (await _v.Dominio.Inscripciones.ListarActivosAsync(comision.Id)).Single(e => e.Apellido == "Cruz");
        await _v.Dominio.Inscripciones.DarDeBajaAsync(cruz.Id, comision.Id, _v.Dominio.Hoy);
        var p = _v.Seguimiento();

        await p.CargarAsync();

        Assert.HasCount(2, p.Filas);
        Assert.IsFalse(p.Filas.Any(f => f.Estudiante.Id == cruz.Id));
    }

    [TestMethod]
    public async Task SinComisionElPanelEstaVacio()
    {
        await _v.Selector.CargarAsync();
        var p = _v.Seguimiento();

        await p.CargarAsync();

        Assert.IsEmpty(p.Filas);
        Assert.IsFalse(p.HayFilas);
    }

    [TestMethod]
    public async Task CA10_2_RegistrarUnContactoSinCompletarAvisaQueFaltaYDejaElFormularioAbierto()
    {
        await ComisionConTresNivelesAsync();
        var p = _v.Seguimiento();
        await p.CargarAsync();
        await Ejecutar(p.Filas[0].ContactarCommand);
        Assert.IsTrue(p.Contacto.FormularioVisible);

        await Ejecutar(p.Contacto.RegistrarContactoCommand);

        Assert.AreEqual("Faltan completar: Resultado, Nota.", p.Contacto.MensajeError);
        Assert.IsTrue(p.Contacto.FormularioVisible);
        Assert.IsEmpty(await _v.Dominio.Repositorio.ListarAsync<Contacto>());
    }

    [TestMethod]
    public async Task CA10_1_YCA11_1_RegistrarUnContactoLoMuestraEnLaFilaConSuFecha()
    {
        await ComisionConTresNivelesAsync();
        var p = _v.Seguimiento();
        await p.CargarAsync();
        await Ejecutar(p.Filas[0].ContactarCommand);
        p.Contacto.FechaContacto = new DateTime(2026, 10, 28);
        p.Contacto.MedioSeleccionado = "Email";
        p.Contacto.ResultadoSeleccionado = "Respondió";
        p.Contacto.Nota = "Va a entregar";

        await Ejecutar(p.Contacto.RegistrarContactoCommand);

        Assert.IsFalse(p.Contacto.FormularioVisible);
        Assert.AreEqual("✔ Contactado el 28/10", p.Filas[0].TextoContacto);
        Assert.IsTrue(p.Filas[0].Contactado);
        Assert.AreEqual("Sin contacto registrado", p.Filas[1].TextoContacto);
        Assert.AreEqual(MedioContacto.Email, (await _v.Dominio.Repositorio.ListarAsync<Contacto>()).Single().Medio);
    }

    [TestMethod]
    public async Task CA11_2_ElFiltroDejaSoloALosDeRiesgoSinContactoEstaSemana()
    {
        await ComisionConTresNivelesAsync();
        var p = _v.Seguimiento();
        await p.CargarAsync();
        await Ejecutar(p.Filas[0].ContactarCommand);
        p.Contacto.FechaContacto = _v.Dominio.Hoy;
        p.Contacto.ResultadoSeleccionado = "Respondió";
        p.Contacto.Nota = "ok";
        await Ejecutar(p.Contacto.RegistrarContactoCommand);
        Assert.AreEqual("No contactados esta semana", p.TextoFiltro);

        await Ejecutar(p.AlternarFiltroCommand);

        Assert.AreEqual("Ver todos", p.TextoFiltro);
        Assert.AreEqual("Bravo, Beto", p.Filas.Single().NombreCompleto);

        await Ejecutar(p.AlternarFiltroCommand);
        Assert.HasCount(3, p.Filas);
    }

    [TestMethod]
    public async Task EnviarEmailAbreElCorreoYDejaElegidoElMedio()
    {
        await ComisionConTresNivelesAsync();
        var p = _v.Seguimiento();
        await p.CargarAsync();
        await Ejecutar(p.Filas[0].ContactarCommand);

        await Ejecutar(p.Contacto.EnviarEmailCommand);

        Assert.IsTrue(_v.Enlaces.Abiertos.Single().StartsWith("mailto:"));
        Assert.AreEqual("Email", p.Contacto.MedioSeleccionado);
    }

    [TestMethod]
    public async Task SiNoSePuedeLlamarSeAvisaYSeMuestraElNumero()
    {
        var comision = await _v.ConComisionAsync();
        var carla = await _v.Estudiantes_ConTelefonoAsync(comision);
        _v.Enlaces.PuedeAbrir = false;
        var p = _v.Seguimiento();
        await p.CargarAsync();
        await Ejecutar(p.Filas.Single().ContactarCommand);

        await Ejecutar(p.Contacto.LlamarCommand);

        Assert.AreEqual("tel:1155550000", _v.Enlaces.Abiertos.Single());
        Assert.IsTrue(p.Contacto.MensajeError.Contains("11-5555-0000"));
        Assert.AreEqual(carla.Id, p.Filas.Single().Estudiante.Id);
    }

    [TestMethod]
    public async Task SinTelefonoElBotonLlamarNoHaceNada()
    {
        await ComisionConTresNivelesAsync();
        var p = _v.Seguimiento();
        await p.CargarAsync();
        await Ejecutar(p.Filas[0].ContactarCommand);

        await Ejecutar(p.Contacto.LlamarCommand);

        Assert.IsFalse(p.Contacto.TieneTelefono);
        Assert.IsEmpty(_v.Enlaces.Abiertos);
    }

    [TestMethod]
    public async Task CancelarCierraElFormulario()
    {
        await ComisionConTresNivelesAsync();
        var p = _v.Seguimiento();
        await p.CargarAsync();
        await Ejecutar(p.Filas[0].ContactarCommand);

        await Ejecutar(p.Contacto.CancelarContactoCommand);

        Assert.IsFalse(p.Contacto.FormularioVisible);
    }
}

internal static class EscenarioDeVistasExtensiones
{
    public static async Task<Estudiante> Estudiantes_ConTelefonoAsync(this EscenarioDeVistas v, Comision comision)
    {
        var estudiante = await v.Dominio.Estudiantes.AltaAsync(
            new DatosEstudiante("L-9", "30999111", "Carla", "Díaz", "c@mail.com", "11-5555-0000"));
        await v.Dominio.Inscripciones.InscribirAsync(estudiante.Id, comision.Id);
        return estudiante;
    }
}
