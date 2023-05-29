using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BCXN.Repositories
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> GetAll();

        Task<TEntity> GetByIdAsync(Guid id);

        Task<List<TEntity>> GetAllAsync();

        Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>,
            IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null);

        Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>,
            IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null);

        Task CreateAsync(TEntity entity, bool isSaved = true);

        Task UpdateAsync(TEntity entity, bool isSaved = true, List<string> excludeFieldNames = null);
        Task DeleteAsync(Guid id, bool isSaved = true);
        Task DeleteRangeAsync(TEntity[] entities, bool isSaved = true);
        Task SaveChanges();
        Task<List<TEntity>> Gets(Expression<Func<TEntity, bool>> spec,
            bool isReadOnly = true,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> preFilter = null,
            params Func<IQueryable<TEntity>, IQueryable<TEntity>>[] postFilters);

        Task<bool> Exist(Expression<Func<TEntity, bool>> spec = null);
        Task<int> Count(Expression<Func<TEntity, bool>> spec = null);
        Task<IEnumerable<TOutput>> GetsAs<TOutput>(Expression<Func<TEntity, TOutput>> projector,
            Expression<Func<TEntity, bool>> spec = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> preFilter = null,
            params Func<IQueryable<TEntity>, IQueryable<TEntity>>[] postFilters);
    }
}
