using System.Net;
using System.Net.Mail;

namespace Symphony.Portal.Web.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            var mailServer = emailSettings["MailServer"];
            var mailPort = int.Parse(emailSettings["MailPort"] ?? "587");
            var senderName = emailSettings["SenderName"];
            var senderEmail = emailSettings["SenderEmail"];
            var password = emailSettings["Password"];

            var client = new SmtpClient(mailServer, mailPort)
            {
                UseDefaultCredentials = false, // Must be set BEFORE Credentials
                Credentials = new NetworkCredential(senderEmail, password),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = false // Plain text for simplicity, or true using simple HTML
            };
            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
        }
    }
}
