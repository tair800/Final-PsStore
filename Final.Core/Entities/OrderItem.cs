using Final.Core.Entities.Common;

namespace Final.Core.Entities
{
    public class OrderItem : BaseEntity
    {
        public int OrderId { get; set; }
        public Order Order { get; set; }


        public int GameId { get; set; }
        public Game Game { get; set; }

        public decimal Price { get; set; }
        public int Quantity { get; set; }

    }

}
