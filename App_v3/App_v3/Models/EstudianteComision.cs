using SQLite;

namespace App_v3.Models;

[Table("EstudianteComision")]
public class EstudianteComision
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int EstudianteId { get; set; }

    [Indexed]
    public int ComisionId { get; set; }
}