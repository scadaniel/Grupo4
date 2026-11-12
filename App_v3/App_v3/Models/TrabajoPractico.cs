// App_v3/Models/TrabajoPractico.cs
using SQLite;

namespace App_v3.Models;

[Table("TrabajosPracticos")]
public class TrabajoPractico
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public string Comision { get; set; } = string.Empty; // 👈 En string para matchear con Clases

    [Indexed]
    public string Nombre { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    public DateTime FechaLimite { get; set; } = DateTime.Today.AddDays(7);
}