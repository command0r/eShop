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
            
            // Ordering evaluators
            if (spec.OrderBy != null)
            {
                query = query.OrderBy(spec.OrderBy);
            }
            
            if (spec.OrderByDescending != null)
            {
                query = query.OrderByDescending(spec.OrderByDescending);
            }
            
            // Pagination evaluator (ordering here is important as paging needs to happen after filter operations, as well as sorting)
            if (spec.IsPagingEnabled)
            {
                query = query.Skip(spec.Skip).Take(spec.Take);
            }

            // Combine all 'include' operations (Aggregate)
            query = spec.Includes.Aggregate(query, (current, include) =>
                current.Include(include));
            
            return query;
        }
    }
}