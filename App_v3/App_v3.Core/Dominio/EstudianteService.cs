namespace App_v3.Dominio;

/// <summary>H1: alta y consulta de estudiantes. Legajo y DNI son únicos en todo el sistema (CA 1.2).</summary>
public class EstudianteService(IRepositorio repositorio)
{
    // CA 1.1
    public async Task<Estudiante> AltaAsync(DatosEstudiante datos)
    {
        ValidacionEstudiante.Validar(datos);
        var nuevo = ValidacionEstudiante.Normalizar(datos);
        await ExigirQueNoExistaAsync(nuevo);
        await repositorio.GuardarAsync(nuevo);
        return nuevo;
    }

    public async Task<List<Estudiante>> ListarAsync() =>
        (await repositorio.ListarAsync<Estudiante>())
            .OrderBy(e => e.Apellido, StringComparer.CurrentCultureIgnoreCase)
            .ThenBy(e => e.Nombre, StringComparer.CurrentCultureIgnoreCase)
            .ToList();

    public async Task<Estudiante?> ObtenerAsync(int id) =>
        (await repositorio.ListarAsync<Estudiante>()).FirstOrDefault(e => e.Id == id);

    /// <summary>Busca por DNI tal como lo escribe el usuario (con o sin puntos). Sirve para inscribir a quien ya está cargado.</summary>
    public async Task<Estudiante?> ObtenerPorDniAsync(string? dni)
    {
        if (string.IsNullOrWhiteSpace(dni)) return null;

        var buscado = ValidacionEstudiante.LimpiarDni(dni);
        return (await repositorio.ListarAsync<Estudiante>()).FirstOrDefault(e => e.Dni == buscado);
    }

    // CA 1.2
    private async Task ExigirQueNoExistaAsync(Estudiante nuevo)
    {
        var existentes = await repositorio.ListarAsync<Estudiante>();

        if (existentes.Any(e => string.Equals(e.Legajo, nuevo.Legajo, StringComparison.OrdinalIgnoreCase)))
            throw new ReglaDeNegocioException(
                $"Ya existe un estudiante con el legajo {nuevo.Legajo}. No se guardó el registro.");

        if (existentes.Any(e => e.Dni == nuevo.Dni))
            throw new ReglaDeNegocioException(
                $"Ya existe un estudiante con el DNI {nuevo.Dni}. No se guardó el registro.");
    }
}
