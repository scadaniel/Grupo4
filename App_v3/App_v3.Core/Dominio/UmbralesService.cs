using System.Globalization;

namespace App_v3.Dominio;

/// <summary>H7: lectura y guardado de los umbrales de riesgo, con el texto tal como lo escribió el usuario.</summary>
public class UmbralesService(IRepositorio repositorio)
{
    public async Task<UmbralesRiesgo> ObtenerAsync() =>
        (await repositorio.ListarAsync<UmbralesRiesgo>()).FirstOrDefault() ?? new UmbralesRiesgo();

    // CA 7.1 y CA 7.3: si algo no es un entero o los valores se contradicen, no se guarda nada.
    public async Task<UmbralesRiesgo> GuardarAsync(
        string? inasistenciasMedio, string? inasistenciasAlto, string? tpsMedio, string? tpsAlto)
    {
        var actual = await ObtenerAsync();
        var nuevos = new UmbralesRiesgo
        {
            Id = actual.Id,
            InasistenciasMedio = Entero(inasistenciasMedio),
            InasistenciasAlto = Entero(inasistenciasAlto),
            TpsNoEntregadosMedio = Entero(tpsMedio),
            TpsNoEntregadosAlto = Entero(tpsAlto)
        };

        if (!nuevos.EsValida(out var mensaje))
            throw new ReglaDeNegocioException(mensaje);

        await repositorio.GuardarAsync(nuevos);
        return nuevos;
    }

    private static int Entero(string? texto) =>
        int.TryParse(texto?.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var valor)
            ? valor
            : throw new ReglaDeNegocioException("Ingresá números enteros en los cuatro umbrales.");
}
