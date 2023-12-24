using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace HttpServer.services
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly Config _config = ServerConfiguration._config;

        public EmailSenderService()
        {

        }

        public void SendEmail(string toEmail, string fromEmail, string subject, string body)
        {
                MailMessage message = new MailMessage(fromEmail, toEmail, subject, body);
                SmtpClient smtp = new SmtpClient(_config.SMTP, _config.SmtpPort);
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential(fromEmail, _config.SenderPassword);
                smtp.Send(message);
        }
    }
}

