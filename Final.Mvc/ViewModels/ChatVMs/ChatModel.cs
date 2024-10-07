namespace Final.Mvc.ViewModels.ChatVMs
{
    public class ChatModel
    {
        public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();

        public string UserName { get; set; }

        public string UserId { get; set; }

        public bool IsAuthenticated { get; set; }
    }
    public class ChatMessage
    {
        public int Id { get; set; }
        public string User { get; set; }
        public string Message { get; set; }
        public string Date { get; set; }
    }
}
