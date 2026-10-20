using SQLite;

namespace App_v3.Models;

public class Persona : IEntidad
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Apellido { get; set; } = string.Empty;

    public string Legajo { get; set; } = string.Empty;

    public string Dni { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Telefono { get; set; } = string.Empty;

    public string Comision { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;

    public RolPersona Rol { get; set; } = RolPersona.Estudiante;

    public DateTime FechaIngreso { get; set; } = DateTime.Now;

    public string EstadoAlerta { get; set; } = "Normal"; // "Normal", "Riesgo Moderado", "Riesgo Alto"

    // Contadores acumulados, recalculados al guardar asistencias y entregas (H6)
    public int Inasistencias { get; set; }

    public int TpsNoEntregados { get; set; }

    // [Ignore]: SQLite no crea esta columna, pero el XAML la puede usar igual.
    [Ignore]
    public string NombreCompleto => $"{Nombre} {Apellido}";
}