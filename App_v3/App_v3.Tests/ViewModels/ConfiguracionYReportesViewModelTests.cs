using App_v3.Dominio;
using App_v3.ViewModels;

namespace App_v3.Tests;

[TestClass]
public sealed class ConfiguracionYReportesViewModelTests
{
    private readonly EscenarioDeVistas _v = new();

    private static Task Ejecutar(System.Windows.Input.ICommand c) => SelectorYMateriasViewModelTests.Ejecutar(c);

    // ---------- Umbrales (H7) ----------

    [TestMethod]
    public async Task CargarMuestraLosUmbralesActuales()
    {
        var p = _v.Umbrales();

        await p.CargarAsync();

        Assert.AreEqual("3", p.InasistenciasMedio);
        Assert.AreEqual("5", p.InasistenciasAlto);
        Assert.AreEqual("1", p.TpsMedio);
        Assert.AreEqual("3", p.TpsAlto);
    }

    [TestMethod]
    public async Task CA7_1_GuardarRegistraLosNuevosValoresYLoAvisa()
    {
        var p = _v.Umbrales();
        p.InasistenciasMedio = "2"; p.InasistenciasAlto = "4"; p.TpsMedio = "2"; p.TpsAlto = "5";

        await Ejecutar(p.GuardarCommand);

        Assert.AreEqual("Umbrales guardados", _v.Dialogos.Ultima.Titulo);
        Assert.AreEqual(4, (await _v.Dominio.Umbrales.ObtenerAsync()).InasistenciasAlto);
    }

    [TestMethod]
    [DataRow("abc", "5", "1", "3")]
    [DataRow("3", "", "1", "3")]
    [DataRow("3", "5", "1", "2.5")]
    public async Task UnValorQueNoEsUnNumeroEnteroSeRechazaYNoDiceQueSeGuardo(string a, string b, string c, string d)
    {
        var p = _v.Umbrales();
        p.InasistenciasMedio = a; p.InasistenciasAlto = b; p.TpsMedio = c; p.TpsAlto = d;

        await Ejecutar(p.GuardarCommand);

        Assert.AreEqual("Atención", _v.Dialogos.Ultima.Titulo);
        Assert.IsTrue(_v.Dialogos.Ultima.Mensaje.Contains("enteros"));
        Assert.IsFalse(_v.Dialogos.Alertas.Any(x => x.Titulo == "Umbrales guardados"));
        Assert.AreEqual(3, (await _v.Dominio.Umbrales.ObtenerAsync()).InasistenciasMedio);
    }

    [TestMethod]
    public async Task CA7_3_LosValoresContradictoriosNoSeGuardan()
    {
        var p = _v.Umbrales();
        p.InasistenciasMedio = "5"; p.InasistenciasAlto = "3"; p.TpsMedio = "1"; p.TpsAlto = "3";

        await Ejecutar(p.GuardarCommand);

        Assert.IsTrue(_v.Dialogos.Ultima.Mensaje.Contains("menor"));
        Assert.IsEmpty(await _v.Dominio.Repositorio.ListarAsync<UmbralesRiesgo>());
    }

    // ---------- Datos de ejemplo ----------

    [TestMethod]
    public async Task CargarDatosDeEjemploPideConfirmacionYDejaListaLaSeleccion()
    {
        var p = new DatosViewModel(new DatosDeEjemplo(_v.Dominio.Repositorio, _v.Dominio.Reloj), _v.Dominio.Repositorio, _v.Selector, _v.Dialogos);

        await Ejecutar(p.CargarEjemploCommand);

        Assert.HasCount(1, _v.Dialogos.Preguntas);
        Assert.IsTrue(_v.Selector.HayComision);
        Assert.HasCount(2, _v.Selector.Materias);
    }

    [TestMethod]
    public async Task SiSeCancelaLaConfirmacionNoSeToucaNada()
    {
        await _v.Dominio.Materias.AltaAsync("Mi materia");
        _v.Dialogos.RespuestaAConfirmar = false;
        var p = new DatosViewModel(new DatosDeEjemplo(_v.Dominio.Repositorio, _v.Dominio.Reloj), _v.Dominio.Repositorio, _v.Selector, _v.Dialogos);

        await Ejecutar(p.CargarEjemploCommand);
        await Ejecutar(p.BorrarTodoCommand);

        Assert.HasCount(1, await _v.Dominio.Materias.ListarAsync());
        Assert.IsEmpty(_v.Dialogos.Alertas);
    }

    [TestMethod]
    public async Task BorrarTodoVaciaLaAplicacion()
    {
        await new DatosDeEjemplo(_v.Dominio.Repositorio, _v.Dominio.Reloj).CargarAsync();
        await _v.Selector.CargarAsync();
        var p = new DatosViewModel(new DatosDeEjemplo(_v.Dominio.Repositorio, _v.Dominio.Reloj), _v.Dominio.Repositorio, _v.Selector, _v.Dialogos);

        await Ejecutar(p.BorrarTodoCommand);

        Assert.IsEmpty(await _v.Dominio.Materias.ListarAsync());
        Assert.IsFalse(_v.Selector.HayComision);
    }

    // ---------- Reportes ----------

    [TestMethod]
    public async Task ElMenuLlevaAlReporteQueSeElige()
    {
        var menu = new MenuReportesViewModel(_v.Navegacion, _v.Dialogos);

        await Ejecutar(menu.IrAsistenciasCommand);
        await Ejecutar(menu.IrEntregasCommand);

        Assert.AreEqual(Rutas.ReporteAsistencias, _v.Navegacion.Rutas[0]);
        Assert.AreEqual(Rutas.ReporteEntregas, _v.Navegacion.Rutas[1]);
    }

    [TestMethod]
    public async Task ElReporteDeAsistenciasMuestraLosRegistrosConSuEstadoYSePuedeVolver()
    {
        var comision = await _v.ConComisionAsync(("Ana", "Gómez"));
        var ana = (await _v.Dominio.Inscripciones.ListarActivosAsync(comision.Id)).Single();
        await _v.Dominio.TomarAsistenciaAsync(await _v.Dominio.NuevaClaseAsync(comision, -2, "Variables"), ana);
        var p = _v.ReporteAsistencias();

        await p.CargarAsync();
        await Ejecutar(p.VolverCommand);

        Assert.AreEqual("Ausente", p.Filas.Single().TextoEstado);
        Assert.AreEqual(EstadoAsistencia.Ausente, p.Filas.Single().Estado);
        Assert.IsTrue(p.Filas.Single().Detalle.Contains("Variables"));
        Assert.AreEqual(1, _v.Navegacion.Vueltas);
    }

    [TestMethod]
    public async Task ElReporteDeAsistenciasFiltraPorDniYLimpiarLoRestablece()
    {
        var comision = await _v.ConComisionAsync(("Ana", "Gómez"), ("Beto", "Pérez"));
        await _v.Dominio.TomarAsistenciaAsync(await _v.Dominio.NuevaClaseAsync(comision, -2));
        var p = _v.ReporteAsistencias();
        await p.CargarAsync();
        Assert.HasCount(2, p.Filas);

        p.Dni = (await _v.Dominio.Estudiantes.ListarAsync()).First().Dni;
        await Ejecutar(p.BuscarCommand);
        Assert.HasCount(1, p.Filas);

        await Ejecutar(p.LimpiarCommand);
        Assert.HasCount(2, p.Filas);
        Assert.AreEqual(string.Empty, p.Dni);
    }

    [TestMethod]
    public async Task ElReporteDeEntregasPermiteVerTodosLosTrabajosOUnoSolo()
    {
        var comision = await _v.ConComisionAsync(("Ana", "Gómez"));
        var primero = await _v.Dominio.NuevoTrabajoAsync(comision, "TP 1", -5);
        await _v.Dominio.NuevoTrabajoAsync(comision, "TP 2", 5);
        var p = _v.ReporteEntregas();

        await p.CargarAsync();
        Assert.HasCount(3, p.Trabajos);
        Assert.HasCount(2, p.Filas);

        p.Trabajo = p.Trabajos.Single(t => t.Trabajo?.Id == primero.Id);
        await Ejecutar(p.BuscarCommand);
        Assert.AreEqual("No entregado", p.Filas.Single().TextoEstado);

        await Ejecutar(p.LimpiarCommand);
        Assert.HasCount(2, p.Filas);
    }

    // ---------- Textos y colores ----------

    [TestMethod]
    public void CadaEstadoTieneSuColorSemantico()
    {
        Assert.AreEqual(PaletaSemantica.Peligro, PaletaSemantica.ClaveDe(NivelRiesgo.Alto));
        Assert.AreEqual(PaletaSemantica.Advertencia, PaletaSemantica.ClaveDe(NivelRiesgo.Medio));
        Assert.AreEqual(PaletaSemantica.Exito, PaletaSemantica.ClaveDe(NivelRiesgo.Bajo));
        Assert.AreEqual(PaletaSemantica.Exito, PaletaSemantica.ClaveDe(EstadoAsistencia.Presente));
        Assert.AreEqual(PaletaSemantica.Advertencia, PaletaSemantica.ClaveDe(EstadoAsistencia.AusenteJustificado));
        Assert.AreEqual(PaletaSemantica.Peligro, PaletaSemantica.ClaveDe(EstadoAsistencia.Ausente));
        Assert.AreEqual(PaletaSemantica.Exito, PaletaSemantica.ClaveDe(EstadoEntrega.Entregado));
        Assert.AreEqual(PaletaSemantica.Peligro, PaletaSemantica.ClaveDe(EstadoEntrega.NoEntregado));
        Assert.AreEqual(PaletaSemantica.Neutro, PaletaSemantica.ClaveDe(EstadoEntrega.Pendiente));
        Assert.AreEqual(PaletaSemantica.Exito, PaletaSemantica.ClaveDe(true));
        Assert.AreEqual(PaletaSemantica.Peligro, PaletaSemantica.ClaveDe(false));
        Assert.AreEqual(PaletaSemantica.Neutro, PaletaSemantica.ClaveDe(null));
    }

    [TestMethod]
    public void LosTextosDeLosEstadosSonLosDeLasHistorias()
    {
        Assert.AreEqual("ALTO", Textos.De(NivelRiesgo.Alto));
        Assert.AreEqual("Presente", Textos.De(EstadoAsistencia.Presente));
        Assert.AreEqual("Ausente justificado", Textos.De(EstadoAsistencia.AusenteJustificado));
        Assert.AreEqual("Entregado", Textos.De(EstadoEntrega.Entregado));
        Assert.AreEqual("No entregado", Textos.De(EstadoEntrega.NoEntregado));
        Assert.AreEqual("Llamada", Textos.De(MedioContacto.Llamada));
        Assert.AreEqual("Email", Textos.De(MedioContacto.Email));
    }
}
