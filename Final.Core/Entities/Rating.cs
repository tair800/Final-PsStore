using Final.Core.Entities.Common;

namespace Final.Core.Entities
{
    public class Rating : BaseEntity
    {
        public int GameId { get; set; }
        public Game Game { get; set; }
        public string UserId { get; set; }
        public User User { get; set; } // Reference to the user entity

        public int Score { get; set; }
    }
}
