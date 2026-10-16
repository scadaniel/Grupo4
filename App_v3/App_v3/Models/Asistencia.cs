using SQLite;

namespace App_v3.Models;

[Table("Asistencias")]
public class Asistencia
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int ClaseId { get; set; }

    [Indexed]
    public int EstudianteId { get; set; }

    public bool Presente { get; set; } // true = Presente, false = Ausente

    public bool Justificada { get; set; } = false; // H4 (CA 4.2): Permite justificar inasistencias

    public DateTime Fecha { get; set; } = DateTime.Today;
}