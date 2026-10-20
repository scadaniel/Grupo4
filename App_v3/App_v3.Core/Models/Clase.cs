using SQLite;

namespace App_v3.Models;

[Table("Clases")]
public class Clase : IEntidad
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string NombreMateria { get; set; } = string.Empty;

    public string Comision { get; set; } = string.Empty;

    // --- Campos para el Calendario de Clases ---
    public DateTime Fecha { get; set; } = DateTime.Today; // Fecha programada de la clase

    public int NumeroClase { get; set; } = 1;             // Número secuencial de clase

    public string Tema { get; set; } = string.Empty;      // Tema o contenido a dictar en la clase

    [Ignore]
    public string DescripcionCompleta => $"{NombreMateria} - [{Comision}] (Clase #{NumeroClase}: {Tema})";

    public override string ToString()
    {
        return DescripcionCompleta;
    }
}