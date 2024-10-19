using Final.Core.Entities.Common;

namespace Final.Core.Entities
{
    public class Dlc : BaseEntity
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public double Price { get; set; }
        public int GameId { get; set; }

        public Game Game { get; set; }
        public ICollection<BasketGame> BasketGames { get; set; }


    }
}
