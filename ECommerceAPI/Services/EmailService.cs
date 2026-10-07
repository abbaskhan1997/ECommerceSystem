using System.Net;
using System.Net.Mail;

namespace ECommerceAPI.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;


        public EmailService (IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendEmail (string to, string subject, string body)
        {
            var email = _configuration["EmailSettings:Email"];
            var password = _configuration["EmailSettings:Password"];

            var smtp = new SmtpClient(
                _configuration["EmailSettings:SmtpServer"],
                int.Parse(_configuration["EmailSettings:Port"]!)
            );

            smtp.Credentials = new NetworkCredential(email, password);
            smtp.EnableSsl = true;

            var message = new MailMessage(
                email,
                to,
                subject,
                body
            );

            smtp.Send(message);
        }
    }
}