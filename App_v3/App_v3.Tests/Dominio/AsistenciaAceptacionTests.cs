using App_v3.Dominio;

namespace App_v3.Tests;


// ============================================================
// PRUEBA DE ACEPTACIÓN – REGISTRO Y CONSULTA DE ASISTENCIAS
//
// Objetivo:
// Verificar que el sistema permita registrar las asistencias de los estudiantes de una comisión y consultar posteriormente
// la planilla, conservando correctamente los estados registrados.
//
// Tipo de prueba:
// Funcional de aceptación, porque comprueba el comportamiento del sistema desde la perspectiva del usuario.
//
// Resultado esperado:
// La planilla indica que la asistencia ya fue registrada, muestra a Ana como presente y a Beto como ausente.
//
// Requisito que valida:
// H3 – El sistema debe permitir registrar la asistencia de los estudiantes de una clase, indicando quiénes están presentes y quiénes ausentes,
// y conservar esos estados al consultar nuevamente la planilla.
//
// COMANDO PARA EJECUTAR DESDE POWERSHELL:
// dotnet test ..\App_v3.Tests\App_v3.Tests.csproj --filter "FullyQualifiedName~AsistenciaAceptacionTests"
//
// ============================================================


[TestClass]
public sealed class AsistenciaAceptacionTests
{
    private readonly EscenarioDePrueba _e = new();

    [TestMethod]
    public async Task RegistrarYConsultarAsistencia_ConservaLosEstadosDeLosEstudiantes()
    {
        // Arrange
        var comision = await _e.NuevaComisionAsync();
        var ana = await _e.NuevoInscriptoAsync(comision, "Ana", "Gómez");
        var beto = await _e.NuevoInscriptoAsync(comision, "Beto", "Pérez");
        var clase = await _e.NuevaClaseAsync(comision, -1);

        // Act: Ana queda presente y Beto ausente.
        await _e.Asistencias.GuardarAsync(
            clase.Id,
            [
                new LineaAsistencia(ana, Presente: true),
                new LineaAsistencia(beto, Presente: false)
            ]);

        // Se vuelve a consultar la planilla.
        var planilla = await _e.Asistencias.ObtenerPlanillaAsync(clase.Id);

        // Assert
        Assert.IsTrue(planilla.YaRegistrada);
        Assert.HasCount(2, planilla.Lineas);

        Assert.IsTrue(
            planilla.Lineas.Single(l => l.Estudiante.Id == ana.Id).Presente);

        Assert.IsFalse(
            planilla.Lineas.Single(l => l.Estudiante.Id == beto.Id).Presente);
    }
}
