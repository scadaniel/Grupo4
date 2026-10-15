using SQLite;

namespace App_v3.Models;

[Table("Clases")]
public class Clase
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string NombreMateria { get; set; } = string.Empty;

    public string Comision { get; set; } = string.Empty;

    // --- Nuevos campos para cumplir con H2 (Calendario de Clases - CA 2.1) ---
    public DateTime Fecha { get; set; } = DateTime.Today; // Fecha programada de la clase

    public int NumeroClase { get; set; } = 1;             // Número secuencial de clase

    public string Tema { get; set; } = string.Empty;      // Tema o contenido a dictar en la clase
}