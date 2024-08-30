using Final.Core.Entities.Common;

namespace Final.Core.Entities
{
    public class Basket : BaseEntity
    {
        public string UserId { get; set; }
        public User User { get; set; }

        public ICollection<BasketGame> BasketGames { get; set; }
    }
}
