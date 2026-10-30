using App_v3.Dominio;
using App_v3.ViewModels;

namespace App_v3.Tests;

[TestClass]
public sealed class SelectorYMateriasViewModelTests
{
    private readonly EscenarioDeVistas _v = new();

    [TestMethod]
    public async Task SinMateriasNoHayComisionYElResumenExplicaQueHacer()
    {
        await _v.Selector.CargarAsync();

        Assert.IsFalse(_v.Selector.HayComision);
        Assert.IsNull(_v.Selector.ComisionId);
        Assert.IsTrue(_v.Selector.Resumen.Contains("Cree una materia"));
    }

    [TestMethod]
    public async Task AlCargarQuedanElegidasLaPrimeraMateriaYLaPrimeraComision()
    {
        var comision = await _v.ConComisionAsync();

        Assert.IsTrue(_v.Selector.HayComision);
        Assert.AreEqual(comision.Id, _v.Selector.ComisionId);
        Assert.HasCount(1, _v.Selector.Materias);
        Assert.HasCount(1, _v.Selector.Comisiones);
    }

    [TestMethod]
    public async Task ElegirOtraMateriaMuestraSusComisionesYAvisaDelCambio()
    {
        var primera = await _v.Dominio.Materias.AltaAsync("Álgebra");
        var segunda = await _v.Dominio.Materias.AltaAsync("Bases de Datos");
        await _v.Dominio.Comisiones.AltaAsync(primera.Id, "1K1");
        var deSegunda = await _v.Dominio.Comisiones.AltaAsync(segunda.Id, "2K1");
        await _v.Selector.CargarAsync();
        var avisos = 0;
        _v.Selector.ComisionCambiada += (_, _) => avisos++;

        _v.Selector.MateriaSeleccionada = _v.Selector.Materias.Single(m => m.Id == segunda.Id);

        Assert.AreEqual(deSegunda.Id, _v.Selector.ComisionId);
        Assert.HasCount(1, _v.Selector.Comisiones);
        Assert.AreEqual(1, avisos);
    }

    [TestMethod]
    public async Task RecargarConservaLaComisionElegida()
    {
        var materia = await _v.Dominio.Materias.AltaAsync("Álgebra");
        await _v.Dominio.Comisiones.AltaAsync(materia.Id, "1K1");
        var segunda = await _v.Dominio.Comisiones.AltaAsync(materia.Id, "1K2");
        await _v.Selector.CargarAsync();
        _v.Selector.ComisionSeleccionada = _v.Selector.Comisiones.Single(c => c.Id == segunda.Id);

        await _v.Selector.CargarAsync();

        Assert.AreEqual(segunda.Id, _v.Selector.ComisionId);
    }

    [TestMethod]
    public async Task CrearUnaMateriaLaDejaElegida()
    {
        var pantalla = _v.Materias();
        pantalla.NombreMateria = "Programación";

        await Ejecutar(pantalla.CrearMateriaCommand);

        Assert.AreEqual("Programación", _v.Selector.MateriaSeleccionada?.Nombre);
        Assert.AreEqual(string.Empty, pantalla.NombreMateria);
    }

    [TestMethod]
    public async Task CrearUnaComisionSinMateriaPideCrearLaMateriaPrimero()
    {
        var pantalla = _v.Materias();
        pantalla.NombreComision = "1K1";

        await Ejecutar(pantalla.CrearComisionCommand);

        Assert.AreEqual("Atención", _v.Dialogos.Ultima.Titulo);
        Assert.IsTrue(_v.Dialogos.Ultima.Mensaje.Contains("materia"));
    }

    [TestMethod]
    public async Task CrearUnaComisionLaDejaElegidaDentroDeLaMateria()
    {
        var pantalla = _v.Materias();
        pantalla.NombreMateria = "Programación";
        await Ejecutar(pantalla.CrearMateriaCommand);
        pantalla.NombreComision = "1K1";

        await Ejecutar(pantalla.CrearComisionCommand);

        Assert.AreEqual("1K1", _v.Selector.ComisionSeleccionada?.Nombre);
        Assert.IsEmpty(_v.Dialogos.Alertas);
    }

    [TestMethod]
    public async Task UnNombreDeComisionRepetidoSeAvisaYNoSeCrea()
    {
        var pantalla = _v.Materias();
        pantalla.NombreMateria = "Programación";
        await Ejecutar(pantalla.CrearMateriaCommand);
        pantalla.NombreComision = "1K1";
        await Ejecutar(pantalla.CrearComisionCommand);
        pantalla.NombreComision = "1k1";

        await Ejecutar(pantalla.CrearComisionCommand);

        Assert.IsTrue(_v.Dialogos.Ultima.Mensaje.Contains("Ya existe"));
        Assert.HasCount(1, _v.Selector.Comisiones);
    }

    internal static Task Ejecutar(System.Windows.Input.ICommand comando)
    {
        var tarea = ((ComandoAsync)comando).EjecutarAsync();
        return tarea;
    }
}
