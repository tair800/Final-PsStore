using Final.Core.Entities.Common;

namespace Final.Core.Entities
{
    public class BasketGame : BaseEntity
    {
        public int BasketId { get; set; }
        public Basket Basket { get; set; }

        public int GameId { get; set; }
        public Game Game { get; set; }

        public int Quantity { get; set; }

        public decimal Sum { get; set; }
    }
}
