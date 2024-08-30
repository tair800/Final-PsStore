using Final.Core.Entities.Common;

namespace Final.Core.Entities
{
    public class Dlc : BaseEntity
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int GameId { get; set; }
        public Game Game { get; set; }
    }
}
