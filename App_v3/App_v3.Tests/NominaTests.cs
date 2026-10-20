using App_v3.Core;

namespace App_v3.Tests;

[TestClass]
public sealed class NominaTests
{
    private static FichaEstudiante Ficha(string legajo, string dni) =>
        new(legajo, dni, "Juan", "Pérez", "juan@mail.com");

    [TestMethod]
    public void CA1_1_AltaDeEstudianteApareceEnLaLista()
    {
        var nomina = new Nomina();

        nomina.Alta(Ficha("L1", "111"));

        Assert.HasCount(1, nomina.Todos);
        Assert.AreEqual("L1", nomina.Todos[0].Legajo);
    }

    [TestMethod]
    public void CA1_1_TelefonoEsOpcional()
    {
        var nomina = new Nomina();

        nomina.Alta(new FichaEstudiante("L1", "111", "Juan", "Pérez", "juan@mail.com"));

        Assert.IsNull(nomina.Todos[0].Telefono);
    }

    [TestMethod]
    [DataRow("", "111", "Juan", "Pérez", "a@b.com")]
    [DataRow("L1", " ", "Juan", "Pérez", "a@b.com")]
    [DataRow("L1", "111", "", "Pérez", "a@b.com")]
    [DataRow("L1", "111", "Juan", "", "a@b.com")]
    [DataRow("L1", "111", "Juan", "Pérez", "")]
    public void CA1_1_AltaSinCamposObligatoriosLanzaExcepcion(
        string legajo, string dni, string nombre, string apellido, string email)
    {
        var nomina = new Nomina();

        Assert.Throws<ArgumentException>(
            () => nomina.Alta(new FichaEstudiante(legajo, dni, nombre, apellido, email)));
        Assert.HasCount(0, nomina.Todos);
    }

    [TestMethod]
    public void CA1_2_RechazaLegajoDuplicado()
    {
        var nomina = new Nomina();
        nomina.Alta(Ficha("L1", "111"));

        Assert.Throws<EstudianteDuplicadoException>(() => nomina.Alta(Ficha("L1", "222")));
        Assert.HasCount(1, nomina.Todos);
    }

    [TestMethod]
    public void CA1_2_RechazaDniDuplicado()
    {
        var nomina = new Nomina();
        nomina.Alta(Ficha("L1", "111"));

        Assert.Throws<EstudianteDuplicadoException>(() => nomina.Alta(Ficha("L2", "111")));
        Assert.HasCount(1, nomina.Todos);
    }

    [TestMethod]
    public void CA1_3_ListaTodosLosEstudiantesRegistrados()
    {
        var nomina = new Nomina();
        nomina.Alta(Ficha("L1", "111"));
        nomina.Alta(Ficha("L2", "222"));

        Assert.HasCount(2, nomina.Todos);
    }

    [TestMethod]
    public void CA1_4_InactivarNoBorraAlEstudiante()
    {
        var nomina = new Nomina();
        nomina.Alta(Ficha("L1", "111"));
        nomina.Alta(Ficha("L2", "222"));

        nomina.Inactivar("L1");

        Assert.HasCount(2, nomina.Todos);          // sigue en el sistema
        Assert.HasCount(1, nomina.Activos);        // pero ya no está activo
        Assert.AreEqual("L2", nomina.Activos[0].Legajo);
    }

    [TestMethod]
    public void CA1_4_InactivarLegajoInexistenteLanzaExcepcion()
    {
        var nomina = new Nomina();

        Assert.Throws<KeyNotFoundException>(() => nomina.Inactivar("L99"));
    }
}