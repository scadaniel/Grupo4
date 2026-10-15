using SQLite;

namespace App_v3.Models;

public class Profesor
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string NombreCompleto { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Catedra { get; set; } = string.Empty;
}
