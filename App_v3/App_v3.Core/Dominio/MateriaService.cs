namespace App_v3.Dominio;

/// <summary>Alta y listado de materias.</summary>
public class MateriaService(IRepositorio repositorio)
{
    public async Task<Materia> AltaAsync(string? nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ReglaDeNegocioException("Complete el nombre de la materia.");

        var materia = new Materia { Nombre = nombre.Trim() };
        var existentes = await repositorio.ListarAsync<Materia>();

        if (existentes.Any(m => string.Equals(m.Nombre, materia.Nombre, StringComparison.OrdinalIgnoreCase)))
            throw new ReglaDeNegocioException($"Ya existe la materia {materia.Nombre}.");

        await repositorio.GuardarAsync(materia);
        return materia;
    }

    public async Task<List<Materia>> ListarAsync() =>
        (await repositorio.ListarAsync<Materia>())
            .OrderBy(m => m.Nombre, StringComparer.CurrentCultureIgnoreCase)
            .ToList();
}
