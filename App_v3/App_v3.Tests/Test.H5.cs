using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

[TestClass]
public class RegistroTrabajosTests
{
    // Función de ejemplo que simula el registro de entrega
    private string RegistrarEntrega(Dictionary<string, string> entregas, string estudiante, string estado)
    {
        entregas[estudiante] = estado;
        return entregas[estudiante];
    }

    // Función de ejemplo que simula la obtención de estudiantes activos
    private List<string> ObtenerEstudiantesActivos()
    {
        return new List<string> { "Juan Pérez", "María Gómez", "Lucas Fernández" };
    }

    [TestMethod]
    public void CA5_1_RegistrarEntregaDeUnTP()
    {
        // Arrange
        var trabajo = new Dictionary<string, string>();

        // Act
        string resultado = RegistrarEntrega(trabajo, "Juan Pérez", "Entregó");

        // Assert
        Assert.AreEqual("Entregó", resultado);
        Assert.AreEqual("Entregó", trabajo["Juan Pérez"]);
    }

    [TestMethod]
    public void CA5_2_RegistrarNoEntregaDeUnTP()
    {
        // Arrange
        var trabajo = new Dictionary<string, string>();

        // Act
        RegistrarEntrega(trabajo, "Juan Pérez", "No entregó");

        // Assert
        Assert.AreEqual("No entregó", trabajo["Juan Pérez"]);
    }

    [TestMethod]
    public void CA5_3_ListarEstudiantesActivos()
    {
        // Act
        var estudiantes = ObtenerEstudiantesActivos();

        // Assert
        Assert.IsNotNull(estudiantes);
        // Usamos HasCount como lo sugiere MSTest para colecciones
        Assert.HasCount(3, estudiantes); 
        Assert.AreEqual("Juan Pérez", estudiantes[0]);
    }

    [TestMethod]
    public void CA5_4_ConsultarEstadoPreviamenteRegistrado()
    {
        // Arrange
        var trabajo = new Dictionary<string, string>
        {
            { "Juan Pérez", "Entregó" },
            { "María Gómez", "No entregó" }
        };

        // Act & Assert
        Assert.AreEqual("Entregó", trabajo["Juan Pérez"]);
        Assert.AreEqual("No entregó", trabajo["María Gómez"]);
    }
}