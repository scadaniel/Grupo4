namespace App_v3.Dominio;

/// <summary>Materia que se dicta. Cada materia tiene una o más comisiones.</summary>
public class Materia : IEntidad
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}
