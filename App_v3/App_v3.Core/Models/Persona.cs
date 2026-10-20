using SQLite;

namespace App_v3.Models;

public class Persona : IEntidad
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Apellido { get; set; } = string.Empty;

    public string Legajo { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Telefono { get; set; } = string.Empty;

    public RolPersona Rol { get; set; } = RolPersona.Estudiante;

    public DateTime FechaIngreso { get; set; } = DateTime.Now;

    public string EstadoAlerta { get; set; } = "Normal"; // "Normal", "Riesgo Moderado", "Riesgo Alto"

    // [Ignore]: SQLite no crea esta columna, pero el XAML la puede usar igual.
    [Ignore]
    public string NombreCompleto => $"{Nombre} {Apellido}";
}