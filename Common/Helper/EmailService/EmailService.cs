using System.Net.Mail;
using System.Net;

namespace ApplicationDev.Common.Helper.EmailService
{
	public class EmailService
	{
		private readonly ILogger<EmailService> _logger;
		public EmailService(ILogger<EmailService> logger)
		{
			_logger = logger;
		}
		public async Task SendVerificationEmail(string userEmail, string name, string verifyLink)
		{
			string? fromEmail = Environment.GetEnvironmentVariable("EMAIL");
			string? fromPassword = Environment.GetEnvironmentVariable("PASSWORD");
			var smtpClient = new SmtpClient("smtp.gmail.com")
			{
				Port = 587,
				Credentials = new NetworkCredential(fromEmail, fromPassword),
				EnableSsl = true,
			};

			var mailMessage = new MailMessage
			{
				From = new MailAddress(fromEmail),
				Subject = "Please verify your email address",
				Body = $"Hello {name},\nPlease verify your email by clicking on the link below:\n{verifyLink}",
				IsBodyHtml = true
			};
			mailMessage.To.Add(userEmail);

			try
			{
				await smtpClient.SendMailAsync(mailMessage);
			}
			catch (Exception ex)
			{
				// Handle exceptions
				Console.WriteLine("Error sending email: " + ex.Message);
			}
		}
	}
}
