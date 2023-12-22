using System;
namespace HttpServer.services
{
	public interface IEmailSenderService
	{
		void SendEmail(string toEmail, string fromEmail,
			string subject, string message);
	}
}

