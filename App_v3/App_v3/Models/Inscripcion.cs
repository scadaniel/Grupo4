using SQLite;

namespace App_v3.Models;

[Table("Inscripciones")]
public class Inscripcion
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int EstudianteId { get; set; }
    public int ClaseId { get; set; }
}
