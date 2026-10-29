namespace App_v3.Dominio;

/// <summary>H2 CA 2.2: trabajo práctico de una comisión con su fecha límite de entrega.</summary>
public class TrabajoPractico : IEntidad
{
    public int Id { get; set; }
    public int ComisionId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public DateTime FechaLimite { get; set; } = DateTime.Today.AddDays(7);
}
