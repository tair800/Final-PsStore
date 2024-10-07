using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
    private static readonly List<ChatMessage> _messages = new List<ChatMessage>();

    public class ChatMessage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string User { get; set; }
        public string Message { get; set; }
        public string Timestamp { get; set; }
    }

    public async Task SendMessage(string user, string message)
    {
        var timestamp = DateTime.Now.ToString("hh:mm tt");
        var chatMessage = new ChatMessage
        {
            User = user,
            Message = message,
            Timestamp = timestamp
        };

        _messages.Add(chatMessage);

        await Clients.All.SendAsync("ReceiveMessage", chatMessage.Id, user, message, timestamp);
    }

    public async Task DeleteMessage(string messageId)
    {
        var message = _messages.FirstOrDefault(m => m.Id == messageId);
        if (message != null)
        {
            _messages.Remove(message);
            await Clients.All.SendAsync("DeleteMessage", messageId);
        }
    }

    public async Task GetAllMessages()
    {
        await Clients.Caller.SendAsync("ReceiveAllMessages", _messages);
    }

    public override async Task OnConnectedAsync()
    {
        await GetAllMessages();
        await base.OnConnectedAsync();
    }
}
