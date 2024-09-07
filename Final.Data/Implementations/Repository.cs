using Final.Core.Entities.Common;
using Final.Core.Repositories;
using Final.Data.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Final.Data.Implementations
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly FinalDbContext _context;
        private readonly DbSet<T> _table;

        public Repository(FinalDbContext context)
        {
            _context = context;
            _table = context.Set<T>();
        }

        public async Task Commit()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task Create(T entity)
        {
            try
            {
                var result = _context.Entry(entity);
                result.State = EntityState.Added;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task Delete(T entity)
        {
            try
            {
                var result = _context.Entry(entity);
                result.State = EntityState.Deleted;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<List<T>> GetAll(Expression<Func<T, bool>> predicate = null, params string[] includes)
        {
            try
            {
                IQueryable<T> query = _table;
                if (includes.Length > 0)
                {
                    query = GetAllIncludes(includes);
                }
                return predicate == null ? await query.ToListAsync() : await query.Where(predicate).ToListAsync();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<T> GetEntity(Expression<Func<T, bool>> predicate = null, params string[] includes)
        {
            try
            {
                IQueryable<T> query = _table;
                if (includes.Length > 0)
                {
                    query = GetAllIncludes(includes);
                }

                var a = predicate == null ? await query.FirstOrDefaultAsync() : await query.FirstOrDefaultAsync(predicate);

                return a;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> isExists(Expression<Func<T, bool>> predicate = null)
        {
            try
            {
                return predicate is null ? false : await _table.AnyAsync(predicate);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task Update(T entity)
        {
            try
            {
                var result = _context.Entry(entity);
                result.State = EntityState.Modified;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }



        private IQueryable<T> GetAllIncludes(params string[] includes)
        {
            try
            {
                IQueryable<T> query = _table;
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
                return query;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}
