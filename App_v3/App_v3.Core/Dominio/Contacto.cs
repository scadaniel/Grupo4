namespace App_v3.Dominio;

public enum MedioContacto
{
    Llamada,
    Email
}

/// <summary>H10 y H11: contacto realizado con un estudiante en riesgo de una comisión.</summary>
public class Contacto : IEntidad
{
    public int Id { get; set; }
    public int EstudianteId { get; set; }
    public int ComisionId { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Today;
    public MedioContacto Medio { get; set; } = MedioContacto.Llamada;
    public string Resultado { get; set; } = string.Empty;
    public string Nota { get; set; } = string.Empty;
}
