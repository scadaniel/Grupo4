using App_v3.Dominio;
using App_v3.ViewModels;

namespace App_v3.Tests;

/// <summary>H12: lo que dice la pantalla del reporte de estudiantes en riesgo.</summary>
[TestClass]
public sealed class ReporteRiesgoViewModelTests
{
    private readonly EscenarioDeVistas _v = new();

    private static Task Ejecutar(System.Windows.Input.ICommand c) => SelectorYMateriasViewModelTests.Ejecutar(c);

    private ReporteRiesgoViewModel Reporte() => new(_v.Selector, _v.Dominio.Seguimiento, _v.Navegacion, _v.Dialogos);

    [TestMethod]
    public async Task CA1_MuestraCuantosEstanEnRiesgoDeCuantos()
    {
        var comision = await _v.ConComisionAsync(("Ana", "Gómez"), ("Beto", "Pérez"), ("Carla", "Ruiz"));
        var ana = (await _v.Dominio.Inscripciones.ListarActivosAsync(comision.Id)).First();
        foreach (var dias in new[] { -21, -14, -7 })
            await _v.Dominio.TomarAsistenciaAsync(await _v.Dominio.NuevaClaseAsync(comision, dias), ana);
        var p = Reporte();

        await p.CargarAsync();

        Assert.AreEqual("1 de 3 estudiantes en riesgo de abandonar la materia", p.Titular);
    }

    [TestMethod]
    public async Task CA3_SinDatosNoDiceCeroEnRiesgo()
    {
        await _v.ConComisionAsync(("Ana", "Gómez"), ("Beto", "Pérez"));
        var p = Reporte();

        await p.CargarAsync();

        Assert.AreEqual("Todavía no hay datos para calcular el riesgo.", p.Titular);
        Assert.DoesNotContain("0 de", p.Titular);
        Assert.DoesNotContain("en riesgo de abandonar", p.Titular);
    }

    [TestMethod]
    public async Task SinComisionElegidaLaPantallaQuedaVacia()
    {
        var p = Reporte();

        await p.CargarAsync();

        Assert.AreEqual(string.Empty, p.Titular);
        Assert.AreEqual(string.Empty, p.Detalle);
    }

    [TestMethod]
    public async Task ElMenuLlevaAlReporteDeRiesgoYSePuedeVolver()
    {
        var menu = new MenuReportesViewModel(_v.Navegacion, _v.Dialogos);
        var p = Reporte();

        await Ejecutar(menu.IrRiesgoCommand);
        await Ejecutar(p.VolverCommand);

        Assert.AreEqual(Rutas.ReporteRiesgo, _v.Navegacion.Rutas[0]);
        Assert.AreEqual(1, _v.Navegacion.Vueltas);
    }
}
