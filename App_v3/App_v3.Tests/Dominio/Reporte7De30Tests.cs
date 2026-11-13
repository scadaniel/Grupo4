using App_v3.Dominio;

namespace App_v3.Tests;


// ============================================================
// PRUEBA DE ACEPTACIÓN – REPORTE DE RIESGO: 7 DE 30
//
// Objetivo:
// Verificar que el reporte de riesgo de la comisión 1.º A muestre correctamente la cantidad de estudiantes en riesgo
// respecto del total de estudiantes activos.
//
// Tipo de prueba:
// Funcional de aceptación. Comprueba que el resultado del reporte coincida con el resultado esperado por el usuario.
//
// Resultado esperado:
// Estado: Calculado.
// Estudiantes en riesgo: 7.
// Total de estudiantes activos: 30.
// Resultado visible: 7 de 30.
//
// Requisito que valida:
// H8 – El sistema debe mostrar la cantidad de estudiantes en riesgo respecto del total de estudiantes 
// activos de la comisión.
//
// COMANDO PARA EJECUTAR DESDE POWERSHELL:
// dotnet test ..\App_v3.Tests\App_v3.Tests.csproj --filter "FullyQualifiedName~Reporte7De30Tests"
//
// ============================================================

[TestClass]
public sealed class Reporte7De30Tests
{
    private readonly EscenarioDePrueba _e = new();

    [TestMethod]
    public async Task ElReporteMuestra7De30EnPrimeroA()
    {
        // Arrange
        await new DatosDeEjemplo(_e.Repositorio, _e.Reloj).CargarAsync();

        var comisiones = await _e.Comisiones.ListarAsync();
        var comision = comisiones.Single(c => c.Nombre == "1°A");

        // Act
        var resumen = await _e.Seguimiento.ResumenDeRiesgoAsync(comision.Id);

        // Assert
        Assert.AreEqual(EstadoResumenRiesgo.Calculado, resumen.Estado);
        Assert.AreEqual(7, resumen.EnRiesgo);
        Assert.AreEqual(30, resumen.TotalActivos);
    }
}
