public class LoginResponseDto
{
	public string Token { get; set; }
	public string Username { get; set; }
	public string Email { get; set; }
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public string UserId { get; set; }
	public List<string> Roles { get; set; }

}