using Final.Core.Entities;
using Final.Core.Repositories;
using Final.Data.Data;
using Microsoft.EntityFrameworkCore;

namespace Final.Infrastructure.Repositories
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly FinalDbContext _context;

        public ActivityRepository(FinalDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Activ activity)
        {
            await _context.Activities.AddAsync(activity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Activ>> GetRecentActivities(int count)
        {
            return await _context.Activities
                .OrderByDescending(a => a.CreatedDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<Activ> Get(int id)
        {
            return await _context.Activities.FindAsync(id);
        }
    }
}
