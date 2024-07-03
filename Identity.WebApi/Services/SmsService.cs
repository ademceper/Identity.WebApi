using Vonage.Messaging;
using Vonage.Request;
using Vonage;

public class SmsService : ISmsService
{
	private readonly IConfiguration _configuration;

	public SmsService(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	public async Task SendSmsAsync(string to, string message)
	{
		var apiKey = _configuration["Nexmo:ApiKey"];
		var apiSecret = _configuration["Nexmo:ApiSecret"];
		var from = _configuration["Nexmo:From"];

		var credentials = Credentials.FromApiKeyAndSecret(apiKey, apiSecret);
		var client = new VonageClient(credentials);

		var response = await client.SmsClient.SendAnSmsAsync(new SendSmsRequest
		{
			To = to,
			From = from,
			Text = message
		});

		if (response.Messages[0].Status != "0")
		{
			throw new Exception($"Failed to send SMS: {response.Messages[0].ErrorText}");
		}
	}
}