using OpenCart.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OpenCart.Repositories.Repositories.GenericRepository
{
    public interface IGenericRepository<TEntity> where TEntity : Entity
    {
        Task<TEntity> AddAsync(TEntity entity);
        Task<bool> DeleteAsync(Guid id);
        IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "");
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter = null);
        Task<List<TEntity>> GetAllAsync();
        Task<TEntity> GetByIdAsync(Guid id);
        Task<IEnumerable<TEntity>> GetAllByIdsAsync(Guid[] ids);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task<PaginationResponse<TEntity>> GetAllPagedAsync(Expression<Func<TEntity, bool>> filter = null, int page = 1, int pageSize = 25);
        Task<bool> AnyAsync(Guid id);
    }
}
