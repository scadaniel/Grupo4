using SQLite;

namespace App_v3.Models;

[Table("Clases")]
public class Clase
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string NombreMateria { get; set; } = string.Empty;

    public string Comision { get; set; } = string.Empty;
}