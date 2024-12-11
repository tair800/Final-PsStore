using Final.Core.Entities.Common;

namespace Final.Core.Entities
{
    public class Activ : BaseEntity
    {
        public string ActionType { get; set; }
        public string EntityName { get; set; }
        public string Details { get; set; }
        public string PerformedBy { get; set; }
        public string? OldData { get; set; }
        public string? NewData { get; set; }
        public int? EntityId { get; set; }

    }
}
