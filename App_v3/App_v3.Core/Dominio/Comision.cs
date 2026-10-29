namespace App_v3.Dominio;

/// <summary>Comisión de una materia. Tiene su calendario de clases y de trabajos prácticos.</summary>
public class Comision : IEntidad
{
    public int Id { get; set; }
    public int MateriaId { get; set; }
    public string Nombre { get; set; } = string.Empty;
}
