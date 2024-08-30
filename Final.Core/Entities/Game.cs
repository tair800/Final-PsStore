using Final.Core.Entities.Common;

namespace Final.Core.Entities
{
    public class Game : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImgUrl { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public ICollection<GamePlatform> GamePlatforms { get; set; }// Many-to-Many 
        public ICollection<Dlc> Dlcs { get; set; }
        public ICollection<WishlistGame> WishlistGames { get; set; }
        public ICollection<BasketGame> BasketGames { get; set; }


    }
}
