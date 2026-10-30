namespace App_v3.Dominio;

/// <summary>Lo que el responsable completa en el formulario de contacto. Fecha, resultado y nota pueden venir vacíos.</summary>
public record DatosContacto(
    int EstudianteId,
    int ComisionId,
    DateTime? Fecha,
    MedioContacto Medio,
    string? Resultado,
    string? Nota);

/// <summary>H10: registrar el contacto realizado con un estudiante y su resultado.</summary>
public class ContactoService(IRepositorio repositorio, InscripcionService inscripciones)
{
    // CA 10.1 y CA 10.2: guarda el contacto o avisa qué campos faltan y no guarda nada.
    public async Task<Contacto> RegistrarAsync(DatosContacto datos)
    {
        ExigirCamposObligatorios(datos);
        await ExigirInscripcionAsync(datos);

        var contacto = new Contacto
        {
            EstudianteId = datos.EstudianteId,
            ComisionId = datos.ComisionId,
            Fecha = datos.Fecha!.Value.Date,
            Medio = datos.Medio,
            Resultado = datos.Resultado!.Trim(),
            Nota = datos.Nota!.Trim()
        };
        await repositorio.GuardarAsync(contacto);
        return contacto;
    }

    public async Task<List<Contacto>> ListarAsync(int estudianteId, int comisionId) =>
        (await repositorio.ListarAsync<Contacto>())
            .Where(c => c.EstudianteId == estudianteId && c.ComisionId == comisionId)
            .OrderByDescending(c => c.Fecha)
            .ToList();

    private static void ExigirCamposObligatorios(DatosContacto datos)
    {
        var faltantes = new List<string>();
        if (datos.Fecha is null) faltantes.Add("Fecha");
        if (string.IsNullOrWhiteSpace(datos.Resultado)) faltantes.Add("Resultado");
        if (string.IsNullOrWhiteSpace(datos.Nota)) faltantes.Add("Nota");

        if (faltantes.Count > 0)
            throw new ReglaDeNegocioException("Faltan completar: " + string.Join(", ", faltantes) + ".");
    }

    private async Task ExigirInscripcionAsync(DatosContacto datos)
    {
        var nomina = await inscripciones.ListarNominaAsync(datos.ComisionId);
        if (!nomina.Any(n => n.Estudiante.Id == datos.EstudianteId))
            throw new ReglaDeNegocioException("El estudiante no está inscripto en esta comisión.");
    }
}
