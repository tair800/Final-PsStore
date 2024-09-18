namespace Final.Application.Services.Interfaces
{
    public interface IEmailService
    {
        public void SendEmail(List<string> emails, string body, string title, string subject);

    }
}
