using SQLite;

namespace App_v3.Models;

public class Estudiante
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public string Legajo { get; set; } = string.Empty;

    [Indexed]
    public string Dni { get; set; } = string.Empty;

    public string NombreCompleto { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Telefono { get; set; } = string.Empty;

    public string Comision { get; set; } = "Comisión A"; // Clave para agrupar alumnos por comisión

    public bool Activo { get; set; } = true;

    // --- Propiedades para el Motor de Riesgo (H6 y H8) ---
    public int Inasistencias { get; set; } = 0;

    public int TpsNoEntregados { get; set; } = 0;
}