using App_v3.Dominio;

namespace App_v3.Tests;


// ============================================================
// PRUEBAS DE ACEPTACIÓN – ALTA DE ESTUDIANTES Y RECHAZO DE DATOS DUPLICADOS
//
// Objetivo:
// Verificar que el sistema permita dar de alta estudiantes, incorporarlos a la nómina de una comisión y rechazar altas
// cuando el Legajo o el DNI ya se encuentran registrados.
//
// Pruebas incluidas:
// 1. Alta de un estudiante y comprobación de su aparición en la nómina de la comisión.
// 2. Rechazo de un estudiante con Legajo duplicado.
// 3. Rechazo de un estudiante con DNI duplicado.
//
// Tipo de prueba:
// Funcional de aceptación. Comprueba el comportamiento del sistema desde la perspectiva del usuario y las reglas
// de negocio establecidas.
//
// Resultado esperado:
// El estudiante se registra correctamente, aparece en la nómina y el sistema impide guardar registros duplicados.
//
// Requisito que valida:
// H1 – El sistema debe permitir registrar estudiantes con sus datos obligatorios e incorporarlos a la comisión correspondiente.
// Debe validar que no existan Legajos ni DNI duplicados, rechazando el alta cuando alguno de esos datos ya esté registrado.
//
// COMANDO PARA EJECUTAR DESDE POWERSHELL:
// dotnet test ..\App_v3.Tests\App_v3.Tests.csproj --filter "FullyQualifiedName~EstudianteAceptacionTests"
//
// ============================================================

[TestClass]
public sealed class EstudianteAceptacionTests
{
    private readonly EscenarioDePrueba _e = new();

    [TestMethod]
    public async Task PA01_AltaDeEstudiante_ApareceEnLaNominaDeLaComision()
    {
        // Arrange
        var comision = await _e.NuevaComisionAsync();

        // Act
        var estudiante = await _e.NuevoEstudianteAsync(
            "L100", "30123456", "Ana", "Gómez");

        await _e.Inscripciones.InscribirAsync(estudiante.Id, comision.Id);

        var nomina = await _e.Inscripciones.ListarNominaAsync(comision.Id);

        // Assert
        Assert.HasCount(1, nomina);
        Assert.AreEqual(estudiante.Id, nomina[0].Estudiante.Id);
        Assert.AreEqual("L100", nomina[0].Estudiante.Legajo);
        Assert.AreEqual("30123456", nomina[0].Estudiante.Dni);
        Assert.AreEqual("Ana", nomina[0].Estudiante.Nombre);
        Assert.AreEqual("Gómez", nomina[0].Estudiante.Apellido);
        Assert.AreEqual("L100@mail.com", nomina[0].Estudiante.Email);
        Assert.IsTrue(nomina[0].Activo);
    }

    [TestMethod]
    public async Task PA01_LegajoDuplicado_SeRechazaYNoSeGuardaOtroEstudiante()
    {
        // Arrange
        await _e.NuevoEstudianteAsync("L100", "30123456", "Ana", "Gómez");

        // Act
        var ex = await Assert.ThrowsAsync<ReglaDeNegocioException>(
            () => _e.Estudiantes.AltaAsync(
                new DatosEstudiante("L100", "30987654", "Beto", "Pérez", "beto@mail.com")));

        // Assert
        Assert.Contains("legajo", ex.Message);
        Assert.HasCount(1, await _e.Repositorio.ListarAsync<Estudiante>());
    }

    [TestMethod]
    public async Task PA01_DniDuplicado_SeRechazaYNoSeGuardaOtroEstudiante()
    {
        // Arrange
        await _e.NuevoEstudianteAsync("L100", "30123456", "Ana", "Gómez");

        // Act
        var ex = await Assert.ThrowsAsync<ReglaDeNegocioException>(
            () => _e.Estudiantes.AltaAsync(
                new DatosEstudiante("L200", "30123456", "Beto", "Pérez", "beto@mail.com")));

        // Assert
        Assert.Contains("DNI", ex.Message);
        Assert.HasCount(1, await _e.Repositorio.ListarAsync<Estudiante>());
    }
}