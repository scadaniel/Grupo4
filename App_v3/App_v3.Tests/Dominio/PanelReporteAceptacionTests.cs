
using App_v3.Dominio;

namespace App_v3.Tests;


// ============================================================
// PRUEBA DE ACEPTACIÓN  – COHERENCIA ENTRE PANEL Y REPORTE
//
// Objetivo:
// Verificar que el panel de riesgo y el reporte de una misma comisión presenten resultados coherentes.
//
// Tipo de prueba:
// Funcional de aceptación. Comprueba que ambas vistas del sistema reflejen la misma información.
//
// Resultado esperado:
// La cantidad de estudiantes en riesgo del panel coincide con la del reporte, y ambos muestran el mismo total de
// estudiantes activos.
//
//
// Requisito que valida:
// H8 – El sistema debe mostrar el nivel de riesgo de los estudiantes en el panel y presentar un resumen en el reporte.
// La cantidad de estudiantes en riesgo y el total de estudiantes activos deben coincidir en ambas vistas.
//
//
// COMANDO PARA EJECUTAR DESDE POWERSHELL:
// dotnet test ..\App_v3.Tests\App_v3.Tests.csproj --filter "FullyQualifiedName~PanelReporteAceptacionTests"
//
// ============================================================

[TestClass]
public sealed class PanelReporteAceptacionTests
{
    private readonly EscenarioDePrueba _e = new();

    [TestMethod]
    public async Task ElPanelYElReporteMuestranLosMismosResultados()
    {
        // Arrange
        await new DatosDeEjemplo(_e.Repositorio, _e.Reloj).CargarAsync();

        var comisiones = await _e.Comisiones.ListarAsync();
        var comision = comisiones.Single(c => c.Nombre == "1°A");

        // Act
        var resumen = await _e.Seguimiento.ResumenDeRiesgoAsync(comision.Id);
        var panel = await _e.Seguimiento.PanelAsync(comision.Id);

        // Assert
        Assert.AreEqual(EstadoResumenRiesgo.Calculado, resumen.Estado);

        Assert.AreEqual(
            panel.Count(f => f.Nivel != NivelRiesgo.Bajo),
            resumen.EnRiesgo);

        Assert.AreEqual(panel.Count, resumen.TotalActivos);
    }
}
