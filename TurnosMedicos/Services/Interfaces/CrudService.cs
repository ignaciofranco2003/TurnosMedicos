using Microsoft.EntityFrameworkCore;
using TurnosMedicos.Data;

namespace TurnosMedicos.Services.Interfaces
{
    public class CrudService<T> : ICrudService<T> where T : class
    {
        protected readonly AppDbContext _db;
        protected readonly DbSet<T> _set;

        public CrudService(AppDbContext db)
        {
            _db = db;
            _set = _db.Set<T>();
        }

        public Task<List<T>> GetAllAsync()
            => _set.AsNoTracking().ToListAsync();

        public Task<T?> GetByIdAsync(int id)
            => _set.FindAsync(id).AsTask();

        public async Task<T> CreateAsync(T entity)
        {
            _set.Add(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> UpdateAsync(int id, Action<T> applyChanges)
        {
            var entity = await _set.FindAsync(id);
            if (entity is null) return false;

            applyChanges(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _set.FindAsync(id);
            if (entity is null) return false;

            _set.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }
    }

}
