public class PasswordResetCode
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public string Email { get; set; }
	public string Code { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime ExpiryTime { get; set; }
}