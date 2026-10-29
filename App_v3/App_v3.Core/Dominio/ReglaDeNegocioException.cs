namespace App_v3.Dominio;

/// <summary>Error de negocio con un mensaje listo para mostrarle al usuario.</summary>
public class ReglaDeNegocioException(string mensaje) : Exception(mensaje);
