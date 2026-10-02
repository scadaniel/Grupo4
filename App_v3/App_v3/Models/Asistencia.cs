using SQLite;

namespace App_v3.Models;

public class Asistencia
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int ClaseId { get; set; }

    public int EstudianteId { get; set; }

    public bool Presente { get; set; }
}
