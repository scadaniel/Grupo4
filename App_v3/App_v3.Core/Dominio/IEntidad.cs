namespace App_v3.Dominio;

/// <summary>Toda entidad que se guarda tiene un Id. Id = 0 significa "todavía no guardada".</summary>
public interface IEntidad
{
    int Id { get; set; }
}
