using SQLite;

namespace App_v3.Models;

public class Clase
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public DateTime Fecha { get; set; }

    public string Tema { get; set; } = string.Empty;

    public int ProfesorId { get; set; }
}
