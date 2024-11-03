using Domain;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public abstract class EntityRepository<T>(TaskManagerDbContext dbContext)
        : IEntityRepository<T> where T : Entity
    {
        protected readonly TaskManagerDbContext DbContext = dbContext;

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await DbContext.Set<T>()
                .Where(it => it.Id == id)
                .FirstOrDefaultAsync();
        }

        public virtual async Task<List<T>> PageAsync(int offset = 0, int count = int.MaxValue)
        {
            return await DbContext.Set<T>()
                .OrderBy(it => it.Id)
                .Skip(offset)
                .Take(count)
                .ToListAsync();
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            // Not using transactions for the simplicity of the test task
            var newEntity = DbContext.Set<T>().Add(entity).Entity;
            await DbContext.SaveChangesAsync();
            return newEntity;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            // Not using transactions for the simplicity of the test task
            var updatedEntity = DbContext.Set<T>().Update(entity).Entity;
            await DbContext.SaveChangesAsync();
            return updatedEntity;
        }
    }
}

namespace Repositories
{
    public interface IEntityRepository<T> where T : Entity
    {
        Task<T?> GetByIdAsync(int id);
        Task<List<T>> PageAsync(int offset = 0, int count = 20);
        Task<T> CreateAsync(T entity);
        Task<T> UpdateAsync(T entity);
    }
}