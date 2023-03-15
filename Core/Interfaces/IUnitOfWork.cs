using Core.Entities;

namespace Core.Interfaces;

// IDisposable is used to dispose the context once transaction is finished
public interface IUnitOfWork : IDisposable
{
    IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
    
    // Returns the number of changes performed against database 
    Task<int> Complete();
}