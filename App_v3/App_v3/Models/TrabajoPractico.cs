using SQLite;

namespace App_v3.Models;

public class TrabajoPractico
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public DateTime FechaLimite { get; set; }
}
