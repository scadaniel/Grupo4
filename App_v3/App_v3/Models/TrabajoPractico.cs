using SQLite;

namespace App_v3.Models;

[Table("TrabajosPracticos")]
public class TrabajoPractico
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public string Nombre { get; set; } = string.Empty; // CA 2.2: Nombre del TP

    public string Descripcion { get; set; } = string.Empty;

    public DateTime FechaLimite { get; set; } = DateTime.Today.AddDays(7); // CA 2.2: Fecha límite de entrega
}