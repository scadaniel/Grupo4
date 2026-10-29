namespace App_v3.Dominio;

/// <summary>Alta y listado de comisiones. El nombre no se repite dentro de una misma materia.</summary>
public class ComisionService(IRepositorio repositorio)
{
    public async Task<Comision> AltaAsync(int materiaId, string? nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ReglaDeNegocioException("Complete el nombre de la comisión.");

        await ExigirMateriaAsync(materiaId);
        var comision = new Comision { MateriaId = materiaId, Nombre = nombre.Trim() };
        await ExigirNombreLibreAsync(comision);

        await repositorio.GuardarAsync(comision);
        return comision;
    }

    public async Task<List<Comision>> ListarAsync(int? materiaId = null) =>
        (await repositorio.ListarAsync<Comision>())
            .Where(c => materiaId is null || c.MateriaId == materiaId)
            .OrderBy(c => c.Nombre, StringComparer.CurrentCultureIgnoreCase)
            .ToList();

    private async Task ExigirMateriaAsync(int materiaId)
    {
        var materias = await repositorio.ListarAsync<Materia>();
        if (!materias.Any(m => m.Id == materiaId))
            throw new ReglaDeNegocioException("La materia no existe.");
    }

    private async Task ExigirNombreLibreAsync(Comision comision)
    {
        var existentes = await repositorio.ListarAsync<Comision>();
        var repetida = existentes.Any(c => c.MateriaId == comision.MateriaId &&
            string.Equals(c.Nombre, comision.Nombre, StringComparison.OrdinalIgnoreCase));

        if (repetida)
            throw new ReglaDeNegocioException($"Ya existe la comisión {comision.Nombre} en esa materia.");
    }
}
