using Final.Core.Entities;
using Final.Core.Repositories;
using Final.Data.Data;

namespace Final.Data.Implementations
{

    public class RatingRepository : Repository<Rating>, IRatingRepository
    {
        public RatingRepository(FinalDbContext context) : base(context)
        {
        }
    }
}
