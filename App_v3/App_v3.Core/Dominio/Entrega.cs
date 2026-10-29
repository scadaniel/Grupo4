namespace App_v3.Dominio;

/// <summary>H5: si un estudiante entregó o no un trabajo práctico.</summary>
public class Entrega : IEntidad
{
    public int Id { get; set; }
    public int TrabajoPracticoId { get; set; }
    public int EstudianteId { get; set; }
    public bool Entregado { get; set; }
}
