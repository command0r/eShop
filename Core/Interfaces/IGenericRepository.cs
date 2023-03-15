using Core.Entities;
using Core.Specifications;

namespace Core.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> ListAllAsync();
        Task<T> GetEntityWithSpec(ISpecification<T> spec);
        Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);
        
        // Method to count the # of items in the list (for pagination)
        Task<int> CountAsync(ISpecification<T> spec);
        
        // Methods to support updating. These methods are not async as they're not dealing with database update ops
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}