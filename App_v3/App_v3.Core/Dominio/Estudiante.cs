namespace App_v3.Dominio;

/// <summary>
/// H1: persona que cursa. Legajo y DNI son únicos en todo el sistema.
/// Estar activo o inactivo no se guarda acá sino en la Inscripcion, porque
/// un estudiante puede abandonar una comisión y seguir en otra.
/// </summary>
public class Estudiante : IEntidad
{
    public int Id { get; set; }
    public string Legajo { get; set; } = string.Empty;
    public string Dni { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;

    public string NombreCompleto => $"{Nombre} {Apellido}";
}
