using SQLite;

namespace App_v3.Models;

[Table("Clases")]
public class Clase
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string NombreMateria { get; set; } = string.Empty;
    public string Comision { get; set; } = string.Empty;

    // --- Nuevos campos del profesor ---
    public string NombreProfesor { get; set; } = string.Empty;
    public string DniProfesor { get; set; } = string.Empty;
    public string TelefonoProfesor { get; set; } = string.Empty;

    public DateTime Fecha { get; set; } = DateTime.Today;
    public int NumeroClase { get; set; } = 1;
    public string Tema { get; set; } = string.Empty;

    [Ignore]
    public string DescripcionCompleta => $"{NombreMateria} - [{Comision}] (Prof. {NombreProfesor})";

    public override string ToString() => DescripcionCompleta;
}