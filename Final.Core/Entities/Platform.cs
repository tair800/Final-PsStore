using Final.Core.Entities.Common;

namespace Final.Core.Entities
{
    public class Platform : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<GamePlatform> GamePlatforms { get; set; }
    }
}
