public class PasswordResetCode
{
	public int Id { get; set; }
	public string Email { get; set; }
	public string Code { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime ExpiryTime { get; set; }
}