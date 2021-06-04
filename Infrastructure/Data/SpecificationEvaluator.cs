using System.Linq;
using Core.Entities;
using Core.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    // Constraint this type to only 'Entity' classes (BaseEntity)
    public class SpecificationEvaluator<TEntity> where TEntity : BaseEntity
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery,
            ISpecification<TEntity> spec)
        {
            var query = inputQuery;
            
            // Evaluate what's inside the specification
            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria);
            }

            // Combine all 'include' operations (Aggregate)
            query = spec.Includes.Aggregate(query, (current, include) =>
                current.Include(include));
            
            return query;
        }
    }
}