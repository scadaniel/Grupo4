using System.Text.RegularExpressions;

namespace App_v3.Dominio;

/// <summary>Lo que el responsable completa en el formulario de alta. Todo puede venir vacío.</summary>
public record DatosEstudiante(
    string? Legajo,
    string? Dni,
    string? Nombre,
    string? Apellido,
    string? Email,
    string? Telefono = null);

/// <summary>H1 CA 1.1: campos obligatorios y formato de DNI y email.</summary>
internal static class ValidacionEstudiante
{
    private static readonly Regex FormatoDni = new("^[0-9]{7,8}$", RegexOptions.Compiled);
    private static readonly Regex FormatoEmail = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public static void Validar(DatosEstudiante datos)
    {
        var faltantes = CamposFaltantes(datos);
        if (faltantes.Count > 0)
            throw new ReglaDeNegocioException(
                "Complete los campos obligatorios: " + string.Join(", ", faltantes) + ".");

        if (!FormatoDni.IsMatch(LimpiarDni(datos.Dni!)))
            throw new ReglaDeNegocioException("El DNI debe tener solo números (7 u 8 dígitos).");

        if (!FormatoEmail.IsMatch(datos.Email!.Trim()))
            throw new ReglaDeNegocioException(
                "El email no tiene un formato válido (por ejemplo: nombre@dominio.com).");
    }

    public static Estudiante Normalizar(DatosEstudiante datos) => new()
    {
        Legajo = datos.Legajo!.Trim(),
        Dni = LimpiarDni(datos.Dni!),
        Nombre = datos.Nombre!.Trim(),
        Apellido = datos.Apellido!.Trim(),
        Email = datos.Email!.Trim(),
        Telefono = datos.Telefono?.Trim() ?? string.Empty
    };

    private static List<string> CamposFaltantes(DatosEstudiante datos)
    {
        var faltantes = new List<string>();
        Revisar(faltantes, "Legajo", datos.Legajo);
        Revisar(faltantes, "DNI", datos.Dni);
        Revisar(faltantes, "Nombre", datos.Nombre);
        Revisar(faltantes, "Apellido", datos.Apellido);
        Revisar(faltantes, "Email", datos.Email);
        return faltantes;
    }

    private static void Revisar(List<string> faltantes, string campo, string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor)) faltantes.Add(campo);
    }

    // Se aceptan DNI escritos como 30.123.456 o con espacios.
    public static string LimpiarDni(string dni) =>
        dni.Replace(".", string.Empty).Replace(" ", string.Empty).Trim();
}
