using Final.Core.Entities.Common;

namespace Final.Core.Entities
{
    public class GamePlatform : BaseEntity
    {
        public int GameId { get; set; }
        public Game Game { get; set; }
        public int PlatformId { get; set; }
        public Platform Platform { get; set; }
    }
}
