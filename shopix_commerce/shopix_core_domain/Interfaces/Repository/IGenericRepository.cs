using shopix_core_domain.Entities;
using System.Linq.Expressions;

namespace shopix_core_domain.Interfaces.Repository
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(Guid id, bool asNoTracking = true);
        Task<IEnumerable<T>> GetAllAsync(bool asNoTracking = true);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = true);
        Task AddAsync(T entity);
        void Update(T entity);
        Task SoftDeleteAsync(Guid id);
    }
}
