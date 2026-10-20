using App_v3.Models;

namespace App_v3.Services;

public interface IDatabaseService
{
    Task<List<T>> GetAllAsync<T>() where T : class, new();
    Task<int> SaveAsync<T>(T item) where T : class, IEntidad, new();
    Task<int> DeleteAsync<T>(T item) where T : class, new();
    Task ResetAsync();
}