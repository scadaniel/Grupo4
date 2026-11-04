using SQLite;

namespace App_v3.Models;

[Table("IntervencionesContacto")]
public class IntervencionContacto
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int EstudianteId { get; set; }

    [Indexed]
    public int ClaseId { get; set; } // Opcional, para asociarlo a una materia/clase específica

    public DateTime Fecha { get; set; } = DateTime.Today;

    public string Medio { get; set; } = "Llamada"; // "Llamada", "Email", "Reunión"

    public string Resultado { get; set; } = string.Empty; // Notas o acuerdo alcanzado
}