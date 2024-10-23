using Final.Core.Entities.Common;

namespace Final.Core.Entities
{
    public class ChatMessage : BaseEntity
    {
        public string User { get; set; }
        public string Message { get; set; }
        public int ChatRoomId { get; set; }
        public ChatRoom ChatRoom { get; set; }
    }
}
