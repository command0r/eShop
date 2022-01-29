using System;
using System.Collections;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly StoreContext _context;
    private Hashtable _repositories;

    // UoW is going to own a database context
    public UnitOfWork(StoreContext context)
    {
        _context = context;
    }
    
    // IDisposable is used to dispose the context once transaction is finished
    public void Dispose()
    {
        _context.Dispose();
    }

    public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
    {
        // Check if there's anything inside the Hash table
        // ??= coalescing operator from C# 8.0 can be user instead to evaluate an expression for 'null'
        if (_repositories == null) _repositories = new Hashtable();
        var type = typeof(TEntity).Name;
        
        // Verify if the Hash table contains the entity from the 'type' variable
        if (!_repositories.ContainsKey(type))
        {
            var repositoryType = typeof(GenericRepository<>);
            
            // Rather then creating an instance of 'context' when creating repository instance, we'll be passing
            // the 'context' the Unit of Work owns as a parameter into the repository we're creating
            var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)),
                _context);
            
            _repositories.Add(type, repositoryInstance);
        }

        return (IGenericRepository<TEntity>) _repositories[type];
    }

    public async Task<int> Complete()
    {
        return await _context.SaveChangesAsync();
    }
}