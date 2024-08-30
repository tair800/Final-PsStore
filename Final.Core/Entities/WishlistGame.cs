using Final.Core.Entities.Common;

namespace Final.Core.Entities
{
    public class WishlistGame : BaseEntity
    {
        public int WishlistId { get; set; }
        public Wishlist Wishlist { get; set; }

        public int GameId { get; set; }
        public Game Game { get; set; }
    }
}
