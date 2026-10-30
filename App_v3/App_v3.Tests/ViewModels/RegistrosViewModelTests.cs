using App_v3.Dominio;
using App_v3.ViewModels;

namespace App_v3.Tests;

[TestClass]
public sealed class RegistrosViewModelTests
{
    private readonly EscenarioDeVistas _v = new();

    private static Task Ejecutar(System.Windows.Input.ICommand c) => SelectorYMateriasViewModelTests.Ejecutar(c);

    // ---------- Calendario (H2) ----------

    [TestMethod]
    public async Task CA2_1_AgregarUnaClaseApareceEnElCalendario()
    {
        await _v.ConComisionAsync();
        var p = _v.Calendario();
        p.FechaClase = new DateTime(2026, 11, 3);
        p.TemaClase = "Variables";

        await Ejecutar(p.GuardarClaseCommand);

        Assert.AreEqual("Clase 1: Variables", p.Eventos.Single().Descripcion);
        Assert.AreEqual("03/11/2026", p.Eventos.Single().Fecha);
        Assert.AreEqual(string.Empty, p.TemaClase);
    }

    [TestMethod]
    public async Task CA2_2_AgregarUnTrabajoApareceEnElCalendario()
    {
        await _v.ConComisionAsync();
        var p = _v.Calendario();
        p.NombreTrabajo = "TP 1";
        p.FechaLimite = new DateTime(2026, 11, 20);

        await Ejecutar(p.GuardarTrabajoCommand);

        Assert.AreEqual("Trabajo práctico", p.Eventos.Single().Tipo);
        Assert.AreEqual("Entrega: TP 1", p.Eventos.Single().Descripcion);
    }

    [TestMethod]
    public async Task CA2_4_UnaClaseSinTemaPideCompletarLosDatosYNoSeGuarda()
    {
        await _v.ConComisionAsync();
        var p = _v.Calendario();

        await Ejecutar(p.GuardarClaseCommand);

        Assert.IsTrue(_v.Dialogos.Ultima.Mensaje.Contains("Complete los campos obligatorios"));
        Assert.IsEmpty(p.Eventos);
    }

    [TestMethod]
    public async Task CA2_4_UnTrabajoSinNombrePideCompletarLosDatos()
    {
        await _v.ConComisionAsync();
        var p = _v.Calendario();

        await Ejecutar(p.GuardarTrabajoCommand);

        Assert.IsTrue(_v.Dialogos.Ultima.Mensaje.Contains("Nombre"));
    }

    // ---------- Asistencia (H3 y H4) ----------

    [TestMethod]
    public async Task CA3_1_AlElegirLaClaseTodosLosEstudiantesActivosAparecenPresentes()
    {
        var comision = await _v.ConComisionAsync(("Ana", "Gómez"), ("Beto", "Pérez"));
        var clase = await _v.Dominio.NuevaClaseAsync(comision, -1, "Variables");
        var p = _v.Asistencia();
        await p.CargarAsync();

        p.ClaseSeleccionada = p.Clases.Single();

        Assert.HasCount(2, p.Lineas);
        Assert.IsTrue(p.Lineas.All(l => l.Presente));
        Assert.IsTrue(p.MostrarGuardar);
        Assert.IsFalse(p.MostrarModificar);
        Assert.AreEqual(clase.Id, p.ClaseSeleccionada!.Clase.Id);
    }

    [TestMethod]
    public async Task CA3_2_YCA3_3_DesmarcarAUnEstudianteYGuardarLoDejaAusente()
    {
        var comision = await _v.ConComisionAsync(("Ana", "Gómez"), ("Beto", "Pérez"));
        await _v.Dominio.NuevaClaseAsync(comision, -1);
        var p = _v.Asistencia();
        await p.CargarAsync();
        p.ClaseSeleccionada = p.Clases.Single();

        p.Lineas.Single(l => l.Estudiante.Nombre == "Beto").Presente = false;
        await Ejecutar(p.GuardarCommand);

        Assert.AreEqual("Asistencia guardada", _v.Dialogos.Ultima.Titulo);
        var guardadas = await _v.Dominio.Repositorio.ListarAsync<Asistencia>();
        Assert.AreEqual(EstadoAsistencia.Ausente, guardadas.Single(a => a.EstudianteId == p.Lineas.Single(l => l.Estudiante.Nombre == "Beto").Estudiante.Id).Estado);
    }

    [TestMethod]
    public async Task CA3_4_ConUnRegistroExistenteSeAvisaYLaGrillaQuedaSinEditar()
    {
        var comision = await _v.ConComisionAsync(("Ana", "Gómez"));
        var clase = await _v.Dominio.NuevaClaseAsync(comision, -1);
        await _v.Dominio.TomarAsistenciaAsync(clase);
        var p = _v.Asistencia();
        await p.CargarAsync();

        p.ClaseSeleccionada = p.Clases.Single();

        Assert.IsTrue(p.YaRegistrada);
        Assert.IsTrue(p.Aviso.Contains("Ya existe un registro"));
        Assert.IsFalse(p.Lineas[0].Editable);
        Assert.IsFalse(p.MostrarGuardar);
        Assert.IsTrue(p.MostrarModificar);
    }

    [TestMethod]
    public async Task CA4_2_YCA4_3_ModificarHabilitaLaEdicionYGuardarCambiosActualiza()
    {
        var comision = await _v.ConComisionAsync(("Ana", "Gómez"));
        var ana = (await _v.Dominio.Inscripciones.ListarActivosAsync(comision.Id)).Single();
        var clase = await _v.Dominio.NuevaClaseAsync(comision, -1);
        await _v.Dominio.TomarAsistenciaAsync(clase, ana);
        var p = _v.Asistencia();
        await p.CargarAsync();
        p.ClaseSeleccionada = p.Clases.Single();
        Assert.IsFalse(p.Lineas[0].Presente);

        await Ejecutar(p.ModificarCommand);
        Assert.IsTrue(p.Lineas[0].Editable);
        Assert.IsTrue(p.MostrarGuardarCambios);

        p.Lineas[0].Presente = true;
        await Ejecutar(p.GuardarCambiosCommand);

        Assert.AreEqual("La asistencia fue actualizada correctamente.", _v.Dialogos.Ultima.Mensaje);
        Assert.IsTrue((await _v.Dominio.Repositorio.ListarAsync<Asistencia>()).Single().Presente);
        Assert.IsFalse(p.Lineas[0].Editable);
    }

    [TestMethod]
    public async Task CA3_5_SoloSePuedeJustificarUnaAusenciaYSeGuarda()
    {
        var comision = await _v.ConComisionAsync(("Ana", "Gómez"));
        await _v.Dominio.NuevaClaseAsync(comision, -1);
        var p = _v.Asistencia();
        await p.CargarAsync();
        p.ClaseSeleccionada = p.Clases.Single();
        var linea = p.Lineas[0];
        Assert.IsFalse(linea.PuedeJustificar);

        linea.Presente = false;
        Assert.IsTrue(linea.PuedeJustificar);
        linea.Justificada = true;
        await Ejecutar(p.GuardarCommand);

        Assert.AreEqual(EstadoAsistencia.AusenteJustificado, (await _v.Dominio.Repositorio.ListarAsync<Asistencia>()).Single().Estado);
    }

    [TestMethod]
    public async Task VolverAMarcarPresenteQuitaLaJustificacion()
    {
        var linea = new LineaAsistenciaViewModel(new LineaAsistencia(new Estudiante(), Presente: false, Justificada: true), editable: true);

        linea.Presente = true;

        Assert.IsFalse(linea.Justificada);
        Assert.IsFalse(linea.ALinea().Justificada);
        await Task.CompletedTask;
    }

    [TestMethod]
    public async Task GuardarSinElegirClaseAvisa()
    {
        await _v.ConComisionAsync(("Ana", "Gómez"));
        var p = _v.Asistencia();
        await p.CargarAsync();

        await Ejecutar(p.GuardarCommand);

        Assert.IsTrue(_v.Dialogos.Ultima.Mensaje.Contains("Elija una clase"));
    }

    // ---------- Entregas (H5) ----------

    [TestMethod]
    public async Task CA5_1_YCA5_2_SeGuardaQuienEntregoYQuienNo()
    {
        var comision = await _v.ConComisionAsync(("Ana", "Gómez"), ("Beto", "Pérez"));
        await _v.Dominio.NuevoTrabajoAsync(comision, "TP 1", 7);
        var p = _v.Entregas();
        await p.CargarAsync();
        p.TrabajoSeleccionado = p.Trabajos.Single();
        Assert.HasCount(2, p.Lineas);

        p.Lineas.Single(l => l.Estudiante.Nombre == "Ana").Entregado = true;
        await Ejecutar(p.GuardarCommand);

        var guardadas = await _v.Dominio.Repositorio.ListarAsync<Entrega>();
        Assert.HasCount(2, guardadas);
        Assert.AreEqual(1, guardadas.Count(g => g.Entregado));
    }

    [TestMethod]
    public async Task CA5_4_UnTrabajoConRegistroMuestraLoGuardadoYUnAviso()
    {
        var comision = await _v.ConComisionAsync(("Ana", "Gómez"));
        var ana = (await _v.Dominio.Inscripciones.ListarActivosAsync(comision.Id)).Single();
        var trabajo = await _v.Dominio.NuevoTrabajoAsync(comision, "TP 1", 7);
        await _v.Dominio.Entregas.GuardarAsync(trabajo.Id, [new LineaEntrega(ana, true)]);
        var p = _v.Entregas();
        await p.CargarAsync();

        p.TrabajoSeleccionado = p.Trabajos.Single();

        Assert.IsTrue(p.Lineas[0].Entregado);
        Assert.AreEqual("Entregó", p.Lineas[0].TextoEstado);
        Assert.IsTrue(p.Aviso.Contains("ya tiene entregas"));
    }

    [TestMethod]
    public async Task GuardarEntregasSinElegirTrabajoAvisa()
    {
        await _v.ConComisionAsync(("Ana", "Gómez"));
        var p = _v.Entregas();
        await p.CargarAsync();

        await Ejecutar(p.GuardarCommand);

        Assert.IsTrue(_v.Dialogos.Ultima.Mensaje.Contains("Elija un trabajo práctico"));
    }
}
