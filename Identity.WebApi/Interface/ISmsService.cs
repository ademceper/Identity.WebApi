public interface ISmsService
{
	Task SendSmsAsync(string number, string message);
}