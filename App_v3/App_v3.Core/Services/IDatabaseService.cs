using App_v3.Models;

namespace App_v3.Services;

public interface IDatabaseService
{
    Task<List<T>> GetAllAsync<T>() where T : class, new();
    Task<int> SaveAsync<T>(T item) where T : class, IEntidad, new();
    Task<int> DeleteAsync<T>(T item) where T : class, new();
    Task ResetAsync();

    // Clases
    Task<List<Clase>> GetClasesAsync();
    Task<int> SaveClaseAsync(Clase clase);

    // Trabajos prácticos
    Task<List<TrabajoPractico>> GetTrabajosPracticosAsync();
    Task<int> SaveTrabajoPracticoAsync(TrabajoPractico tp);

    // Estudiantes e inscripciones
    Task<List<Persona>> GetEstudiantesPorComisionAsync(string comision);
    Task<List<Persona>> GetEstudiantesPorClaseIdAsync(int claseId);
    Task<bool> InscribirEstudiantePorDniAsync(string dni, int claseId);

    // Asistencias
    Task<List<Asistencia>> GetAsistenciasPorClaseAsync(int claseId);
    Task GuardarListaAsistenciasAsync(int claseId, List<Asistencia> asistencias);

    // Entregas de TPs
    Task<List<EntregaTp>> GetEntregasPorTpAsync(int trabajoPracticoId);
    Task GuardarListaEntregasTpAsync(int trabajoPracticoId, List<EntregaTp> entregas);

    // Reportes
    Task<List<ReporteAsistenciaItem>> ObtenerReporteAsistenciasAsync(
        string? dniFiltro, int? claseIdFiltro, DateTime? fechaInicio, DateTime? fechaFin);

    Task<List<ReporteTpItem>> ObtenerReporteEntregasTpAsync(
        string? dniFiltro, int? trabajoPracticoIdFiltro);
}