using SQLite;

namespace App_v3.Models;

public class EntregaTrabajo
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int TrabajoPracticoId { get; set; }

    public int EstudianteId { get; set; }

    public bool Entregado { get; set; }
}
