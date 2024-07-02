using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Identity.WebApi.Services
{
	public class SmsService : ISmsService
	{
		private readonly IConfiguration _configuration;

		public SmsService(IConfiguration configuration)
		{
			_configuration = configuration;
			TwilioClient.Init(_configuration["Twilio:AccountSID"], _configuration["Twilio:AuthToken"]);
		}

		public async Task SendSmsAsync(string number, string message)
		{
			var messageOptions = new CreateMessageOptions(new PhoneNumber(number))
			{
				From = new PhoneNumber(_configuration["Twilio:PhoneNumber"]),
				Body = message
			};

			var msg = await MessageResource.CreateAsync(messageOptions);
		}
	}
}
