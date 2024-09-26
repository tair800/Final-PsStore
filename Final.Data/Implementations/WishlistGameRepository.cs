using Final.Core.Entities;
using Final.Core.Repositories;
using Final.Data.Data;

namespace Final.Data.Implementations
{
    public class WishlistGameRepository : Repository<WishlistGame>, IWishlistGameRepository
    {
        public WishlistGameRepository(FinalDbContext context) : base(context)
        {
        }
    }

}
