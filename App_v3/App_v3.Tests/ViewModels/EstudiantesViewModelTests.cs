using App_v3.Dominio;
using App_v3.ViewModels;

namespace App_v3.Tests;

[TestClass]
public sealed class EstudiantesViewModelTests
{
    private readonly EscenarioDeVistas _v = new();

    private static void Completar(EstudiantesViewModel p, string legajo = "L-1", string dni = "30123456",
        string nombre = "Ana", string apellido = "Gómez", string email = "ana@mail.com", string telefono = "")
    {
        p.Legajo = legajo; p.Dni = dni; p.Nombre = nombre; p.Apellido = apellido; p.Email = email; p.Telefono = telefono;
    }

    private static Task Ejecutar(System.Windows.Input.ICommand c) => SelectorYMateriasViewModelTests.Ejecutar(c);

    [TestMethod]
    public async Task CA1_1_ElAltaApareceEnLaNominaDeLaComisionYLimpiaElFormulario()
    {
        await _v.ConComisionAsync();
        var p = _v.Estudiantes();
        await p.CargarAsync();
        Completar(p, telefono: "11-5555-1111");

        await Ejecutar(p.GuardarCommand);

        Assert.HasCount(1, p.Nomina);
        Assert.AreEqual("Gómez, Ana", p.Nomina[0].NombreCompleto);
        Assert.AreEqual("Activo", p.Nomina[0].TextoEstado);
        Assert.AreEqual(string.Empty, p.Legajo);
        Assert.AreEqual("Estudiante guardado", _v.Dialogos.Ultima.Titulo);
    }

    [TestMethod]
    public async Task ElDniEscritoSeGuardaYElEstudianteQuedaInscripto()
    {
        var comision = await _v.ConComisionAsync();
        var p = _v.Estudiantes();
        Completar(p, dni: "30.123.456");

        await Ejecutar(p.GuardarCommand);

        var estudiante = (await _v.Dominio.Estudiantes.ListarAsync()).Single();
        Assert.AreEqual("30123456", estudiante.Dni);
        Assert.HasCount(1, await _v.Dominio.Inscripciones.ListarActivosAsync(comision.Id));
    }

    [TestMethod]
    [DataRow("ana.mail.com")]
    [DataRow("ana@mail")]
    [DataRow("")]
    public async Task UnEmailInvalidoSeRechazaYNoSeGuardaNada(string email)
    {
        await _v.ConComisionAsync();
        var p = _v.Estudiantes();
        Completar(p, email: email);

        await Ejecutar(p.GuardarCommand);

        Assert.AreEqual("Atención", _v.Dialogos.Ultima.Titulo);
        Assert.IsEmpty(await _v.Dominio.Estudiantes.ListarAsync());
        Assert.IsEmpty(p.Nomina);
    }

    [TestMethod]
    public async Task UnDniQueNoEsNumericoSeRechaza()
    {
        await _v.ConComisionAsync();
        var p = _v.Estudiantes();
        Completar(p, dni: "abc");

        await Ejecutar(p.GuardarCommand);

        Assert.IsTrue(_v.Dialogos.Ultima.Mensaje.Contains("DNI"));
        Assert.IsEmpty(await _v.Dominio.Estudiantes.ListarAsync());
    }

    [TestMethod]
    public async Task CA1_2_UnLegajoOUnDniRepetidosMuestranUnaAdvertenciaYNoSeGuardan()
    {
        await _v.ConComisionAsync();
        var p = _v.Estudiantes();
        Completar(p);
        await Ejecutar(p.GuardarCommand);

        Completar(p, legajo: "L-1", dni: "40999888", nombre: "Beto", apellido: "Pérez", email: "b@b.com");
        await Ejecutar(p.GuardarCommand);
        Assert.IsTrue(_v.Dialogos.Ultima.Mensaje.Contains("legajo"));

        Completar(p, legajo: "L-2", dni: "30123456", nombre: "Beto", apellido: "Pérez", email: "b@b.com");
        await Ejecutar(p.GuardarCommand);
        Assert.IsTrue(_v.Dialogos.Ultima.Mensaje.Contains("DNI"));

        Assert.HasCount(1, await _v.Dominio.Estudiantes.ListarAsync());
    }

    [TestMethod]
    public async Task SinComisionElegidaElAltaPideElegirUna()
    {
        await _v.Selector.CargarAsync();
        var p = _v.Estudiantes();
        Completar(p);

        await Ejecutar(p.GuardarCommand);

        Assert.IsTrue(_v.Dialogos.Ultima.Mensaje.Contains("Elija una comisión"));
        Assert.IsEmpty(await _v.Dominio.Estudiantes.ListarAsync());
    }

    [TestMethod]
    public async Task UnEstudianteYaCargadoSePuedeInscribirEnOtraComisionPorDni()
    {
        var materia = await _v.Dominio.Materias.AltaAsync("Álgebra");
        var a = await _v.Dominio.Comisiones.AltaAsync(materia.Id, "A");
        var b = await _v.Dominio.Comisiones.AltaAsync(materia.Id, "B");
        var ana = await _v.Dominio.NuevoInscriptoAsync(a);
        await _v.Selector.CargarAsync(materia.Id, b.Id);
        var p = _v.Estudiantes();
        await p.CargarAsync();
        p.DniExistente = ana.Dni;

        await Ejecutar(p.InscribirExistenteCommand);

        Assert.HasCount(1, p.Nomina);
        Assert.HasCount(1, await _v.Dominio.Inscripciones.ListarActivosAsync(b.Id));
        Assert.HasCount(1, await _v.Dominio.Inscripciones.ListarActivosAsync(a.Id));
    }

    [TestMethod]
    public async Task InscribirPorUnDniQueNoExisteAvisa()
    {
        await _v.ConComisionAsync();
        var p = _v.Estudiantes();
        p.DniExistente = "11222333";

        await Ejecutar(p.InscribirExistenteCommand);

        Assert.IsTrue(_v.Dialogos.Ultima.Mensaje.Contains("No hay ningún estudiante"));
    }

    [TestMethod]
    public async Task CA1_4_DarDeBajaPideConfirmacionYConservaAlEstudianteComoInactivo()
    {
        await _v.ConComisionAsync(("Ana", "Gómez"));
        var p = _v.Estudiantes();
        await p.CargarAsync();

        await Ejecutar(p.Nomina[0].CambiarEstadoCommand);

        Assert.HasCount(1, _v.Dialogos.Preguntas);
        Assert.HasCount(1, p.Nomina);
        Assert.IsFalse(p.Nomina[0].Activo);
        Assert.IsTrue(p.Nomina[0].TextoEstado.StartsWith("Inactivo desde"));
        Assert.AreEqual("Reactivar", p.Nomina[0].TextoAccion);
        Assert.HasCount(1, await _v.Dominio.Estudiantes.ListarAsync());
    }

    [TestMethod]
    public async Task SiElUsuarioCancelaLaBajaNadaCambia()
    {
        await _v.ConComisionAsync(("Ana", "Gómez"));
        var p = _v.Estudiantes();
        await p.CargarAsync();
        _v.Dialogos.RespuestaAConfirmar = false;

        await Ejecutar(p.Nomina[0].CambiarEstadoCommand);

        Assert.IsTrue(p.Nomina[0].Activo);
    }

    [TestMethod]
    public async Task CA13_1_ElFiltroMuestraSoloLosInactivosOSoloLosActivos()
    {
        var comision = await _v.ConComisionAsync(("Ana", "Gómez"), ("Beto", "Pérez"));
        var beto = (await _v.Dominio.Inscripciones.ListarActivosAsync(comision.Id)).Single(e => e.Nombre == "Beto");
        await _v.Dominio.Inscripciones.DarDeBajaAsync(beto.Id, comision.Id, _v.Dominio.Hoy);
        var p = _v.Estudiantes();
        await p.CargarAsync();
        Assert.HasCount(2, p.Nomina);

        p.Filtro = EstudiantesViewModel.SoloInactivos;
        Assert.HasCount(1, p.Nomina);
        Assert.AreEqual("Pérez, Beto", p.Nomina[0].NombreCompleto);

        p.Filtro = EstudiantesViewModel.SoloActivos;
        Assert.AreEqual("Gómez, Ana", p.Nomina.Single().NombreCompleto);
    }

    [TestMethod]
    public async Task ReactivarVuelveAHacerActivoAlEstudiante()
    {
        var comision = await _v.ConComisionAsync(("Ana", "Gómez"));
        var ana = (await _v.Dominio.Inscripciones.ListarActivosAsync(comision.Id)).Single();
        await _v.Dominio.Inscripciones.DarDeBajaAsync(ana.Id, comision.Id, _v.Dominio.Hoy);
        var p = _v.Estudiantes();
        await p.CargarAsync();

        await Ejecutar(p.Nomina[0].CambiarEstadoCommand);

        Assert.IsTrue(p.Nomina[0].Activo);
        Assert.AreEqual(0, _v.Dialogos.Preguntas.Count);
    }

    [TestMethod]
    public async Task CA9_1_VerLaFichaMuestraDatosNivelMotivoEstadoYHistorial()
    {
        var comision = await _v.ConComisionAsync(("Ana", "Gómez"));
        var ana = (await _v.Dominio.Inscripciones.ListarActivosAsync(comision.Id)).Single();
        await _v.Dominio.TomarAsistenciaAsync(await _v.Dominio.NuevaClaseAsync(comision, -3), ana);
        await _v.Dominio.NuevoTrabajoAsync(comision, "TP 1", -1);
        var p = _v.Estudiantes();
        await p.CargarAsync();

        await Ejecutar(p.Nomina[0].VerFichaCommand);

        var ficha = p.Ficha!;
        Assert.IsTrue(p.HayFicha);
        Assert.AreEqual("Ana Gómez", ficha.NombreCompleto);
        Assert.AreEqual("MEDIO", ficha.TextoNivel);
        Assert.AreEqual("1 inasistencia y 1 TP vencido sin entregar", ficha.Motivo);
        Assert.AreEqual("Activo", ficha.TextoEstado);
        Assert.AreEqual("Inasistencias: 1 · TPs no entregados: 1", ficha.Resumen);
        Assert.AreEqual("Ausente", ficha.Asistencias.Single().TextoEstado);
        Assert.AreEqual("No entregado", ficha.Entregas.Single().TextoEstado);
    }

    [TestMethod]
    public async Task CA13_2_LaFichaDeUnInactivoNoTieneNivelPeroConservaSusRegistros()
    {
        var comision = await _v.ConComisionAsync(("Ana", "Gómez"));
        var ana = (await _v.Dominio.Inscripciones.ListarActivosAsync(comision.Id)).Single();
        await _v.Dominio.TomarAsistenciaAsync(await _v.Dominio.NuevaClaseAsync(comision, -3), ana);
        await _v.Dominio.Inscripciones.DarDeBajaAsync(ana.Id, comision.Id, _v.Dominio.Hoy);
        var p = _v.Estudiantes();
        await p.CargarAsync();

        await Ejecutar(p.Nomina[0].VerFichaCommand);

        Assert.IsFalse(p.Ficha!.TieneNivel);
        Assert.AreEqual("—", p.Ficha.TextoNivel);
        Assert.IsTrue(p.Ficha.TextoEstado.StartsWith("Inactivo desde"));
        Assert.HasCount(1, p.Ficha.Asistencias);
    }

    [TestMethod]
    public async Task CerrarLaFichaLaOculta()
    {
        await _v.ConComisionAsync(("Ana", "Gómez"));
        var p = _v.Estudiantes();
        await p.CargarAsync();
        await Ejecutar(p.Nomina[0].VerFichaCommand);

        await Ejecutar(p.Ficha!.CerrarCommand);

        Assert.IsFalse(p.HayFicha);
    }

    [TestMethod]
    public async Task ElegirOtraComisionCambiaLaNomina()
    {
        var materia = await _v.Dominio.Materias.AltaAsync("Álgebra");
        var a = await _v.Dominio.Comisiones.AltaAsync(materia.Id, "A");
        var b = await _v.Dominio.Comisiones.AltaAsync(materia.Id, "B");
        await _v.Dominio.NuevoInscriptoAsync(a, "Ana", "Gómez");
        await _v.Dominio.NuevoInscriptoAsync(b, "Beto", "Pérez");
        await _v.Selector.CargarAsync(materia.Id, a.Id);
        var p = _v.Estudiantes();
        await p.CargarAsync();
        Assert.AreEqual("Gómez, Ana", p.Nomina.Single().NombreCompleto);

        _v.Selector.ComisionSeleccionada = _v.Selector.Comisiones.Single(c => c.Id == b.Id);

        Assert.AreEqual("Pérez, Beto", p.Nomina.Single().NombreCompleto);
    }

    // ---------- H10 CA 10.1: registrar un contacto desde la ficha ----------

    private async Task<EstudiantesViewModel> FichaAbiertaAsync()
    {
        await _v.ConComisionAsync(("Ana", "Gómez"));
        var p = _v.Estudiantes();
        await p.CargarAsync();
        await Ejecutar(p.Nomina[0].VerFichaCommand);
        return p;
    }

    [TestMethod]
    public async Task CA10_1_DesdeLaFichaSeRegistraUnContactoYQuedaEnElHistorial()
    {
        var p = await FichaAbiertaAsync();
        Assert.IsEmpty(p.Ficha!.Contactos);

        await Ejecutar(p.Ficha.ContactarCommand);
        Assert.IsTrue(p.Contacto.FormularioVisible);
        Assert.IsTrue(p.Contacto.TituloFormulario.Contains("Ana Gómez"));

        p.Contacto.FechaContacto = new DateTime(2026, 10, 28);
        p.Contacto.ResultadoSeleccionado = "Respondió";
        p.Contacto.Nota = "Va a entregar el TP";
        await Ejecutar(p.Contacto.RegistrarContactoCommand);

        Assert.IsFalse(p.Contacto.FormularioVisible);
        Assert.IsTrue(p.HayFicha);
        Assert.HasCount(1, p.Ficha!.Contactos);
        Assert.IsTrue(p.Ficha.Contactos[0].Contains("Va a entregar el TP"));
        Assert.HasCount(1, await _v.Dominio.Repositorio.ListarAsync<Contacto>());
    }

    [TestMethod]
    public async Task CA10_2_DesdeLaFichaSinResultadoNiNotaIndicaQueFaltaYNoGuarda()
    {
        var p = await FichaAbiertaAsync();
        await Ejecutar(p.Ficha!.ContactarCommand);

        await Ejecutar(p.Contacto.RegistrarContactoCommand);

        Assert.AreEqual("Faltan completar: Resultado, Nota.", p.Contacto.MensajeError);
        Assert.IsTrue(p.Contacto.FormularioVisible);
        Assert.IsEmpty(await _v.Dominio.Repositorio.ListarAsync<Contacto>());
    }

    [TestMethod]
    public async Task ACerrarLaFichaTambienSeCierraElFormularioDeContacto()
    {
        var p = await FichaAbiertaAsync();
        await Ejecutar(p.Ficha!.ContactarCommand);

        await Ejecutar(p.Ficha.CerrarCommand);

        Assert.IsFalse(p.Contacto.FormularioVisible);
    }

    [TestMethod]
    public async Task ADeUnEstudianteDadoDeBajaNoSeLoContactaDesdeLaFicha()
    {
        var comision = await _v.ConComisionAsync(("Ana", "Gómez"));
        var ana = (await _v.Dominio.Inscripciones.ListarActivosAsync(comision.Id)).Single();
        await _v.Dominio.Inscripciones.DarDeBajaAsync(ana.Id, comision.Id, _v.Dominio.Hoy);
        var p = _v.Estudiantes();
        await p.CargarAsync();

        await Ejecutar(p.Nomina[0].VerFichaCommand);

        Assert.IsFalse(p.Ficha!.PuedeContactar);
    }
}
