using App_v3.Models;
using App_v3.Services;

namespace App_v3.Tests;

/// <summary>
/// Base de prueba del calendario: solo los 4 métodos de ICalendarioStore, guardados en memoria.
/// No depende de IDatabaseService, así que no se rompe si esa interfaz crece.
/// </summary>
public sealed class CalendarioStoreEnMemoria : ICalendarioStore
{
    private readonly List<Clase> _clases = new();
    private readonly List<TrabajoPractico> _trabajos = new();
    private int _ultimoId;

    public IReadOnlyList<Clase> ClasesGuardadas => _clases;
    public IReadOnlyList<TrabajoPractico> TrabajosGuardados => _trabajos;

    public Task<List<Clase>> GetClasesAsync() => Task.FromResult(_clases.ToList());

    public Task<int> SaveClaseAsync(Clase clase)
    {
        if (clase.Id == 0)
        {
            clase.Id = ++_ultimoId;
            _clases.Add(clase);
        }
        return Task.FromResult(1);
    }

    public Task<List<TrabajoPractico>> GetTrabajosPracticosAsync() => Task.FromResult(_trabajos.ToList());

    public Task<int> SaveTrabajoPracticoAsync(TrabajoPractico tp)
    {
        if (tp.Id == 0)
        {
            tp.Id = ++_ultimoId;
            _trabajos.Add(tp);
        }
        return Task.FromResult(1);
    }
}
