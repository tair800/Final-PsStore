using Final.Core.Entities;
using Final.Core.Repositories;
using Final.Data.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Final.Data.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly FinalDbContext _context;
        private readonly DbSet<User> _table;

        public UserRepository(FinalDbContext context)
        {
            _context = context;
            _table = context.Set<User>();
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

        public async Task Create(User entity)
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

        public async Task Delete(User entity)
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

        public async Task<List<User>> GetAll(Expression<Func<User, bool>> predicate = null, params string[] includes)
        {
            try
            {
                IQueryable<User> query = _table;
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

        public async Task<User> GetEntity(Expression<Func<User, bool>> predicate = null, params string[] includes)
        {
            try
            {
                IQueryable<User> query = _table;
                if (includes.Length > 0)
                {
                    query = GetAllIncludes(includes);
                }

                return predicate == null ? await query.FirstOrDefaultAsync() : await query.FirstOrDefaultAsync(predicate);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> IsExists(Expression<Func<User, bool>> predicate = null)
        {
            try
            {
                return predicate == null ? false : await _table.AnyAsync(predicate);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<bool> isExists(Expression<Func<User, bool>> predicate = null)
        {
            throw new NotImplementedException();
        }

        public Task<List<User>> Search(Expression<Func<User, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public async Task Update(User entity)
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

        private IQueryable<User> GetAllIncludes(params string[] includes)
        {
            try
            {
                IQueryable<User> query = _table;
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
