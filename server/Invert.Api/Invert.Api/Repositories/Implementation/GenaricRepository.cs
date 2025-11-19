using Invert.Api.Data;
using Invert.Api.Entities;
using Invert.Api.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;

namespace Invert.Api.Repositories.Implementation
{
    public class GenaricRepository<T> : IGenaricRepository<T> where T : class
    {
        private readonly ApplicationDbContext _dbContext;
        DbSet<T> dbSet;
        public GenaricRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            dbSet = _dbContext.Set<T>();
        }
        public async Task Add(T entity)
        {
            await dbSet.AddAsync(entity);
        }

        public bool Any(Expression<Func<T, bool>> filter)
        {
            return dbSet.Any(filter);
        }

        public async Task<T> Get(Expression<Func<T, bool>> filter, string? includeProperty = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrEmpty(includeProperty))
            {
                foreach (var include in includeProperty.Split([',']
                , StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(include);
                }
            }
            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperty = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (!string.IsNullOrEmpty(includeProperty))
            {
                foreach (var include in includeProperty.Split([',']
                , StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(include);
                }
            }
            return await query.AsNoTracking().ToListAsync();
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }
    }
}