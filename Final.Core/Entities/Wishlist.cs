using Final.Core.Entities.Common;

namespace Final.Core.Entities
{
    public class Wishlist : BaseEntity
    {
        public string UserId { get; set; }

        public User User { get; set; }

        public ICollection<WishlistGame> WishlistGames { get; set; } = new List<WishlistGame>();

    }
}
