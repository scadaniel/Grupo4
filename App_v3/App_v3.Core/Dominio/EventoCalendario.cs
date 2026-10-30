namespace App_v3.Dominio;

public enum TipoEvento
{
    Clase,
    TrabajoPractico
}

/// <summary>H2 CA 2.3: un renglón del calendario de la comisión, sea clase o trabajo práctico.</summary>
public record EventoCalendario(TipoEvento Tipo, int Id, DateTime Fecha, string Descripcion);
