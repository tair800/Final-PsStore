using Final.Application.Services.Interfaces;
using System.Net;
using System.Net.Mail;

namespace Final.Application.Services.Implementations
{
    public class EmailService : IEmailService
    {
        public void SendEmail(List<string> emails, string body, string title, string subject)
        {
            MailMessage mail = new();
            mail.From = new MailAddress("code.final@outlook.com", "PlaystationAze");
            foreach (var email in emails)
            {
                mail.To.Add(new MailAddress(email));
            }
            mail.Subject = subject;
            mail.IsBodyHtml = true;
            mail.Body = body;

            SmtpClient smtpClient = new()
            {
                Host = "smtp-mail.outlook.com", // Use Outlook's SMTP server
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential("code.final@outlook.com", "mhwwfxncpitphspw!"),
            };
            smtpClient.Send(mail);
        }
    }
}
