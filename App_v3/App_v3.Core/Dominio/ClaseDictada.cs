namespace App_v3.Dominio;

/// <summary>
/// Clase de las historias H2, H3 y H4: una fecha del calendario de la comisión.
/// Se llama ClaseDictada para no confundirla con la materia ni con la comisión.
/// </summary>
public class ClaseDictada : IEntidad
{
    public int Id { get; set; }
    public int ComisionId { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Today;
    public int Numero { get; set; } = 1;
    public string Tema { get; set; } = string.Empty;
}
