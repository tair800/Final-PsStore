using Final.Core.Entities.Common;

namespace Final.Core.Entities
{
    public class Setting : BaseEntity
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
