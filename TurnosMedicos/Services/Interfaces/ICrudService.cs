namespace TurnosMedicos.Services.Interfaces
{
    public interface ICrudService<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<T> CreateAsync(T entity);
        Task<bool> UpdateAsync(int id, Action<T> applyChanges);
        Task<bool> DeleteAsync(int id);
    }
}
