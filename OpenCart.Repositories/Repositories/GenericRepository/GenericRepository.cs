using Microsoft.EntityFrameworkCore;
using OpenCart.Common;
using OpenCart.Infrastructure.Context;
using OpenCart.Repositories.Repositories.GenericRepository;
using System.Linq.Expressions;

namespace OpenCart.Repositories.Repositories
{
    public class GenericRepository<TEntity> : IDisposable, IGenericRepository<TEntity> where TEntity : Entity
    {
        private readonly OpenCartDbContext _dbContext;

        public GenericRepository(OpenCartDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public virtual IEnumerable<TEntity> Get(
         Expression<Func<TEntity, bool>> filter = null,
         Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
         string includeProperties = "")
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }
        public async Task<TEntity> GetByIdAsync(Guid id)
        {
            return await _dbContext.Set<TEntity>().FindAsync(id);
        }

        public async Task<List<TEntity>> GetAllAsync()
        {
            return await _dbContext.Set<TEntity>().ToListAsync();
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
                return false;

            _dbContext.Set<TEntity>().Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<PaginationResponse<TEntity>> GetAllPagedAsync(Expression<Func<TEntity, bool>> filter = null, int page = 1, int pageSize = 25)
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>();

            if (filter != null)
            {
                query = query.Where(filter);
            }
            var totalCount = await query.CountAsync();

            var data = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginationResponse<TEntity>
            {
                Data = data,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<IEnumerable<TEntity>> GetAllByIdsAsync(Guid[] ids)
        {
            var response = new List<TEntity>();
            foreach (var id in ids)
            {
                response.Add(await GetByIdAsync(id));
            }
            return response;
        }
        public async Task<bool> AnyAsync(Guid id) 
        {
            return await GetByIdAsync(id) != null;
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>();
            var result = filter != null ? await query.FirstOrDefaultAsync(filter) : await query.FirstOrDefaultAsync();
            return result;
        }
    }

}
