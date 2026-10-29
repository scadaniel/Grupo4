using App_v3.Core;
using App_v3.Models;

namespace App_v3.Tests;

// Base de prueba para los contactos: los guarda en una lista, sin SQLite.
public sealed class ContactoStoreEnMemoria : IContactoStore
{
    private readonly List<Contacto> _contactos = new();

    public IReadOnlyList<Contacto> Guardados => _contactos;

    public Task GuardarAsync(Contacto contacto)
    {
        _contactos.Add(contacto);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Contacto>> ListarAsync() =>
        Task.FromResult<IReadOnlyList<Contacto>>(_contactos.ToList());
}

[TestClass]
public sealed class ContactosTests
{
    // Día fijo para que los resultados no cambien según cuándo se corran las pruebas.
    private static readonly DateTime Hoy = new(2026, 10, 29);

    private static SeguimientoContactos Crear(ContactoStoreEnMemoria store) =>
        new(store, () => Hoy);

    private static NuevoContacto Valido(int estudianteId = 1, DateTime? fecha = null) =>
        new(estudianteId, fecha ?? Hoy, MedioContacto.Llamada,
            "Respondió", "Se comprometió a entregar el TP 2.");

    // Historia H10: registrar un contacto con un estudiante.

    [TestMethod]
    public async Task CA10_1_ContactoValidoQuedaEnElHistorial()
    {
        var store = new ContactoStoreEnMemoria();

        await Crear(store).RegistrarAsync(Valido(estudianteId: 7));

        Assert.HasCount(1, store.Guardados);
        Assert.AreEqual(7, store.Guardados[0].EstudianteId);
        Assert.AreEqual(MedioContacto.Llamada, store.Guardados[0].Medio);
    }

    [TestMethod]
    [DataRow(false, "Respondió", "Hablamos del TP", "Fecha")]
    [DataRow(true, "", "Hablamos del TP", "Resultado")]
    [DataRow(true, "   ", "Hablamos del TP", "Resultado")]
    [DataRow(true, "Respondió", "", "Nota")]
    public async Task CA10_2_SinFechaResultadoONotaNoSeGuarda(
        bool conFecha, string resultado, string nota, string campoEsperado)
    {
        var store = new ContactoStoreEnMemoria();
        var nuevo = new NuevoContacto(
            1, conFecha ? Hoy : (DateTime?)null, MedioContacto.Email, resultado, nota);

        var ex = await Assert.ThrowsAsync<ContactoInvalidoException>(
            () => Crear(store).RegistrarAsync(nuevo));

        Assert.Contains(campoEsperado, ex.CamposFaltantes);
        Assert.IsEmpty(store.Guardados);
    }

    [TestMethod]
    public async Task CA10_2_AvisaTodosLosCamposQueFaltan()
    {
        var store = new ContactoStoreEnMemoria();
        var vacio = new NuevoContacto(1, null, MedioContacto.Llamada, null, null);

        var ex = await Assert.ThrowsAsync<ContactoInvalidoException>(
            () => Crear(store).RegistrarAsync(vacio));

        Assert.HasCount(3, ex.CamposFaltantes);
    }

    // Historia H11: mostrar en el panel a quién ya se contactó y cuándo.

    [TestMethod]
    public async Task CA11_1_UltimoContactoEsElMasReciente()
    {
        var store = new ContactoStoreEnMemoria();
        var seguimiento = Crear(store);
        await seguimiento.RegistrarAsync(Valido(1, Hoy.AddDays(-10)));
        await seguimiento.RegistrarAsync(Valido(1, Hoy.AddDays(-2)));
        await seguimiento.RegistrarAsync(Valido(2, Hoy.AddDays(-1)));

        var ultimo = await seguimiento.UltimoContactoAsync(1);

        Assert.AreEqual((DateTime?)Hoy.AddDays(-2), ultimo);
    }

    [TestMethod]
    public async Task CA11_1_SinContactosElUltimoContactoEsNulo()
    {
        var ultimo = await Crear(new ContactoStoreEnMemoria()).UltimoContactoAsync(1);

        Assert.IsNull(ultimo);
    }

    [TestMethod]
    public async Task CA11_1_UltimosContactosDevuelveUnaFechaPorEstudiante()
    {
        var store = new ContactoStoreEnMemoria();
        var seguimiento = Crear(store);
        await seguimiento.RegistrarAsync(Valido(1, Hoy.AddDays(-10)));
        await seguimiento.RegistrarAsync(Valido(1, Hoy.AddDays(-2)));
        await seguimiento.RegistrarAsync(Valido(2, Hoy.AddDays(-1)));

        var ultimos = await seguimiento.UltimosContactosAsync();

        Assert.HasCount(2, ultimos);
        Assert.AreEqual(Hoy.AddDays(-2), ultimos[1]);
        Assert.AreEqual(Hoy.AddDays(-1), ultimos[2]);
    }

    [TestMethod]
    public async Task CA11_2_FiltroMuestraSoloEnRiesgoSinContactoReciente()
    {
        var store = new ContactoStoreEnMemoria();
        var seguimiento = Crear(store);
        await seguimiento.RegistrarAsync(Valido(1, Hoy.AddDays(-2)));   // contactado esta semana
        await seguimiento.RegistrarAsync(Valido(2, Hoy.AddDays(-10)));  // contacto viejo
        var panel = new[]
        {
            (EstudianteId: 1, Nivel: NivelRiesgo.Alto),
            (EstudianteId: 2, Nivel: NivelRiesgo.Alto),
            (EstudianteId: 3, Nivel: NivelRiesgo.Medio),
            (EstudianteId: 4, Nivel: NivelRiesgo.Bajo)
        };

        var resultado = await seguimiento.NoContactadosEstaSemanaAsync(panel);

        CollectionAssert.AreEqual(new[] { 2, 3 }, resultado);
    }

    [TestMethod]
    [DataRow(7, false)]
    [DataRow(8, true)]
    public async Task CA11_2_ElLimiteSonSieteDias(int diasAtras, bool apareceEnElFiltro)
    {
        var store = new ContactoStoreEnMemoria();
        var seguimiento = Crear(store);
        await seguimiento.RegistrarAsync(Valido(1, Hoy.AddDays(-diasAtras)));

        var resultado = await seguimiento.NoContactadosEstaSemanaAsync(
            new[] { (EstudianteId: 1, Nivel: NivelRiesgo.Alto) });

        Assert.AreEqual(apareceEnElFiltro, resultado.Contains(1));
    }
}
