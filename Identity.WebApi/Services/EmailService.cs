using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace Identity.WebApi.Services
{
	public class EmailService : IEmailService
	{
		private readonly IConfiguration _configuration;

		/// <summary>
		/// Initializes a new instance of the <see cref="EmailService"/> class.
		/// </summary>
		/// <param name="configuration">The application configuration settings.</param>
		public EmailService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		/// <summary>
		/// Sends an email asynchronously.
		/// </summary>
		/// <param name="to">The recipient email address.</param>
		/// <param name="subject">The subject of the email.</param>
		/// <param name="body">The body of the email.</param>
		/// <returns>A task that represents the asynchronous email send operation.</returns>
		public async Task SendEmailAsync(string to, string subject, string body)
		{
			var emailMessage = new MimeMessage();
			emailMessage.From.Add(new MailboxAddress("Dream", _configuration["EmailSettings:From"]));
			emailMessage.To.Add(new MailboxAddress("", to));
			emailMessage.Subject = subject;

			var builder = new BodyBuilder
			{
				HtmlBody = $@"
                <!DOCTYPE html>
                <html lang='en'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>{subject}</title>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            background-color: #f4f4f4;
                            margin: 0;
                            padding: 0;
                        }}
                        .container {{
                            background-color: #ffffff;
                            margin: 50px auto;
                            padding: 20px;
                            max-width: 600px;
                            border-radius: 10px;
                            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
                        }}
                        .header {{
                            background-color: #007bff;
                            color: #ffffff;
                            padding: 10px;
                            text-align: center;
                            border-radius: 10px 10px 0 0;
                        }}
                        .header h1 {{
                            margin: 0;
                        }}
                        .content {{
                            padding: 20px;
                        }}
                        .content p {{
                            line-height: 1.6;
                        }}
                        .footer {{
                            background-color: #007bff;
                            color: #ffffff;
                            padding: 10px;
                            text-align: center;
                            border-radius: 0 0 10px 10px;
                        }}
                        .btn {{
                            display: inline-block;
                            padding: 10px 20px;
                            margin: 20px 0;
                            background-color: #007bff;
                            color: #ffffff;
                            text-decoration: none;
                            border-radius: 5px;
                        }}
                        .btn:hover {{
                            background-color: #0056b3;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>{subject}</h1>
                        </div>
                        <div class='content'>
                            <p>{body}</p>
                        </div>
                        <div class='footer'>
                            <p>&copy; 2024 Dream. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>"
			};

			emailMessage.Body = builder.ToMessageBody();

			using (var client = new SmtpClient())
			{
				// Connect to the SMTP server using secure connection
				await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
				// Authenticate using the configured email credentials
				await client.AuthenticateAsync(_configuration["EmailSettings:Username"], _configuration["EmailSettings:Password"]);
				// Send the email
				await client.SendAsync(emailMessage);
				// Disconnect from the SMTP server
				await client.DisconnectAsync(true);
			}
		}
	}
}
