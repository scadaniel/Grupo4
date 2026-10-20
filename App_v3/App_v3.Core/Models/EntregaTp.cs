using SQLite;

namespace App_v3.Models;

[Table("EntregasTp")]
public class EntregaTp : IEntidad
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int TrabajoPracticoId { get; set; }

    [Indexed]
    public int EstudianteId { get; set; }

    public bool Entregado { get; set; } = false; // true = Entregado, false = Pendiente/No entregado

    public DateTime? FechaEntrega { get; set; }
}
