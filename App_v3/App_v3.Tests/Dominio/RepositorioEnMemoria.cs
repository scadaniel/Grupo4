using System.Text.Json;
using App_v3.Dominio;

namespace App_v3.Tests;

/// <summary>
/// Repositorio de las pruebas: guarda copias, igual que una base de datos real,
/// así un servicio que modifica un objeto sin guardarlo no pasa desapercibido.
/// </summary>
public sealed class RepositorioEnMemoria : IRepositorio
{
    private readonly Dictionary<Type, List<IEntidad>> _tablas = [];

    public Task<List<T>> ListarAsync<T>() where T : class, IEntidad =>
        Task.FromResult(Tabla<T>().Select(e => Copiar((T)e)).ToList());

    public Task GuardarAsync<T>(T entidad) where T : class, IEntidad
    {
        var tabla = Tabla<T>();
        if (entidad.Id == 0)
            entidad.Id = tabla.Count == 0 ? 1 : tabla.Max(e => e.Id) + 1;

        tabla.RemoveAll(e => e.Id == entidad.Id);
        tabla.Add(Copiar(entidad));
        return Task.CompletedTask;
    }

    private List<IEntidad> Tabla<T>()
    {
        if (!_tablas.TryGetValue(typeof(T), out var tabla))
            _tablas[typeof(T)] = tabla = [];
        return tabla;
    }

    private static T Copiar<T>(T entidad) =>
        JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(entidad))!;
}
