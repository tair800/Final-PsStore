using Final.Core.Entities.Common;

namespace Final.Core.Entities
{
    public class ChatRoom : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<ChatMessage> Messages { get; set; }
    }
}
