using App_v3.Services;

namespace App_v3.Tests;

[TestClass]
public sealed class CalendarioServiceTests
{
    private static readonly DateTime Fecha = new(2026, 10, 20);

    private static async Task<T> LanzaAsync<T>(Func<Task> accion) where T : Exception
    {
        try
        {
            await accion();
        }
        catch (T ex)
        {
            return ex;
        }

        Assert.Fail($"Se esperaba una excepción {typeof(T).Name}.");
        return null!;
    }

    // ---------------- CA 2.1: carga de clases ----------------

    [TestMethod]
    public async Task CA2_1_AgregarUnaClaseLaDejaRegistradaConSuTemaYFecha()
    {
        var store = new CalendarioStoreEnMemoria();
        var servicio = new CalendarioService(store);

        await servicio.AgregarEventoAsync("Clase", "Introducción a Sensores", Fecha);

        Assert.AreEqual(1, store.ClasesGuardadas.Count);
        Assert.AreEqual("Introducción a Sensores", store.ClasesGuardadas[0].Tema);
        Assert.AreEqual(Fecha, store.ClasesGuardadas[0].Fecha);
    }

    [TestMethod]
    public async Task CA2_1_LasClasesSeNumeranEnOrdenDeCarga()
    {
        var store = new CalendarioStoreEnMemoria();
        var servicio = new CalendarioService(store);

        await servicio.AgregarEventoAsync("Clase", "Primera", Fecha);
        await servicio.AgregarEventoAsync("Clase", "Segunda", Fecha.AddDays(7));

        Assert.AreEqual(1, store.ClasesGuardadas[0].NumeroClase);
        Assert.AreEqual(2, store.ClasesGuardadas[1].NumeroClase);
    }

    [TestMethod]
    public async Task CA2_1_ElTemaSeGuardaSinEspaciosSobrantes()
    {
        var store = new CalendarioStoreEnMemoria();
        var servicio = new CalendarioService(store);

        await servicio.AgregarEventoAsync("Clase", "   Motores   ", Fecha);

        Assert.AreEqual("Motores", store.ClasesGuardadas[0].Tema);
    }

    [TestMethod]
    public async Task CA2_1_VariasClasesElMismoDiaSeAceptanPorqueHayVariasComisiones()
    {
        var store = new CalendarioStoreEnMemoria();
        var servicio = new CalendarioService(store);

        await servicio.AgregarEventoAsync("Clase", "Comisión A: Sensores", Fecha);
        await servicio.AgregarEventoAsync("Clase", "Comisión B: Sensores", Fecha);

        Assert.AreEqual(2, store.ClasesGuardadas.Count);
    }

    // ---------------- CA 2.2: carga de trabajos prácticos ----------------

    [TestMethod]
    public async Task CA2_2_AgregarUnTrabajoLoAnadeConSuNombreYFechaLimite()
    {
        var store = new CalendarioStoreEnMemoria();
        var servicio = new CalendarioService(store);

        await servicio.AgregarEventoAsync("Trabajo Práctico", "TP 1", Fecha);

        Assert.AreEqual(1, store.TrabajosGuardados.Count);
        Assert.AreEqual("TP 1", store.TrabajosGuardados[0].Nombre);
        Assert.AreEqual(Fecha, store.TrabajosGuardados[0].FechaLimite);
    }

    // ---------------- CA 2.3: visualización del calendario ----------------

    [TestMethod]
    public async Task CA2_3_ClasesYTrabajosSeListanJuntosEnOrdenCronologico()
    {
        var store = new CalendarioStoreEnMemoria();
        var servicio = new CalendarioService(store);
        var f1 = new DateTime(2026, 10, 10);
        var f2 = new DateTime(2026, 10, 20);
        var f3 = new DateTime(2026, 10, 30);

        await servicio.AgregarEventoAsync("Trabajo Práctico", "TP 1", f3);
        await servicio.AgregarEventoAsync("Clase", "Clase B", f2);
        await servicio.AgregarEventoAsync("Clase", "Clase A", f1);

        var eventos = await servicio.ListarEventosAsync();

        CollectionAssert.AreEqual(new[] { f1, f2, f3 }, eventos.Select(e => e.Fecha).ToArray());
        CollectionAssert.AreEqual(
            new[] { "Clase", "Clase", "Trabajo Práctico" },
            eventos.Select(e => e.Tipo).ToArray());
    }

    [TestMethod]
    public async Task CA2_3_LaDescripcionDeUnaClaseIncluyeNumeroYTema()
    {
        var servicio = new CalendarioService(new CalendarioStoreEnMemoria());

        await servicio.AgregarEventoAsync("Clase", "Introducción a Sensores", Fecha);

        var eventos = await servicio.ListarEventosAsync();

        Assert.AreEqual("Clase 1: Introducción a Sensores", eventos[0].Descripcion);
    }

    // ---------------- CA 2.4: validación de datos obligatorios ----------------

    [TestMethod]
    [DataRow("Clase", "")]
    [DataRow("Clase", "   ")]
    [DataRow("Trabajo Práctico", "")]
    [DataRow("Trabajo Práctico", null)]
    public async Task CA2_4_SinTituloNoSeGuardaNada(string tipo, string? titulo)
    {
        var store = new CalendarioStoreEnMemoria();
        var servicio = new CalendarioService(store);

        await LanzaAsync<DatosInvalidosException>(() => servicio.AgregarEventoAsync(tipo, titulo, Fecha));

        Assert.AreEqual(0, store.ClasesGuardadas.Count);
        Assert.AreEqual(0, store.TrabajosGuardados.Count);
    }

    [TestMethod]
    [DataRow(null, "Tema")]
    [DataRow("", "Tema")]
    [DataRow("  ", "Tema")]
    public async Task CA2_4_SinTipoNoSeGuardaNada(string? tipo, string titulo)
    {
        var store = new CalendarioStoreEnMemoria();
        var servicio = new CalendarioService(store);

        await LanzaAsync<DatosInvalidosException>(() => servicio.AgregarEventoAsync(tipo, titulo, Fecha));

        Assert.AreEqual(0, store.ClasesGuardadas.Count);
        Assert.AreEqual(0, store.TrabajosGuardados.Count);
    }

    [TestMethod]
    public async Task CA2_4_ElMensajeSolicitaCompletarLosCamposObligatorios()
    {
        var servicio = new CalendarioService(new CalendarioStoreEnMemoria());

        var ex = await LanzaAsync<DatosInvalidosException>(() => servicio.AgregarEventoAsync("Clase", "", Fecha));

        Assert.IsTrue(ex.Message.Contains("campos obligatorios"));
    }

    [TestMethod]
    public async Task UnTipoDeEventoDesconocidoSeRechazaYNoSeGuardaNada()
    {
        var store = new CalendarioStoreEnMemoria();
        var servicio = new CalendarioService(store);

        await LanzaAsync<DatosInvalidosException>(() => servicio.AgregarEventoAsync("Examen", "Parcial", Fecha));

        Assert.AreEqual(0, store.ClasesGuardadas.Count);
        Assert.AreEqual(0, store.TrabajosGuardados.Count);
    }
}
