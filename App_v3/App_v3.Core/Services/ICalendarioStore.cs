using App_v3.Models;

namespace App_v3.Services;

/// <summary>
/// Lo mínimo que el calendario necesita de la base de datos.
/// Al depender solo de esto, agregar métodos a IDatabaseService no rompe las pruebas.
/// </summary>
public interface ICalendarioStore
{
    Task<List<Clase>> GetClasesAsync();
    Task<int> SaveClaseAsync(Clase clase);
    Task<List<TrabajoPractico>> GetTrabajosPracticosAsync();
    Task<int> SaveTrabajoPracticoAsync(TrabajoPractico tp);
}

/// <summary>Adaptador: usa la base de datos real (IDatabaseService) como ICalendarioStore.</summary>
public class CalendarioStoreSobreBaseDeDatos : ICalendarioStore
{
    private readonly IDatabaseService _db;

    public CalendarioStoreSobreBaseDeDatos(IDatabaseService db) => _db = db;

    public Task<List<Clase>> GetClasesAsync() => _db.GetClasesAsync();
    public Task<int> SaveClaseAsync(Clase clase) => _db.SaveClaseAsync(clase);
    public Task<List<TrabajoPractico>> GetTrabajosPracticosAsync() => _db.GetTrabajosPracticosAsync();
    public Task<int> SaveTrabajoPracticoAsync(TrabajoPractico tp) => _db.SaveTrabajoPracticoAsync(tp);
}
