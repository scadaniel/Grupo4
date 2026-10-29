using SQLite;

namespace App_v3.Models;

[Table("Estudiante")]
public class Persona
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Apellido { get; set; } = string.Empty;

    public string Legajo { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Telefono { get; set; } = string.Empty;

    public string Rol { get; set; } = string.Empty; // "Estudiante" o "Docente"

    public DateTime FechaIngreso { get; set; } = DateTime.Now;

    public string EstadoAlerta { get; set; } = "Normal"; // "Normal", "Riesgo Moderado", "Riesgo Alto"

    // [Ignore] le indica a SQLite que NO cree esta columna física en la tabla,
    // pero tu interfaz XAML la puede seguir usando normalmente para mostrar el nombre completo.
    [Ignore]
    public string NombreCompleto => $"{Nombre} {Apellido}";
}