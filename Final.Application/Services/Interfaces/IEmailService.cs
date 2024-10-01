namespace Final.Application.Services.Interfaces
{
    public interface IEmailService
    {
        public void SendEmailOld(List<string> emails, string body, string title, string subject);
        public void SendEmail(string from, string to, string subject, string body, string smtpHost, int smtpPort, bool enableSsl, string smtpUser, string smtpPass);
        public Task SendEmailAsyncToManyPeople(string from, List<string> recipients, string subject, string body, string smtpHost, int smtpPort, bool enableSsl, string smtpUser, string smtpPass);

    }
}
