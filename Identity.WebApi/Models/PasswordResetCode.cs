/// <summary>
/// Represents a password reset code for a user.
/// </summary>
public class PasswordResetCode
{
	/// <summary>
	/// Gets or sets the unique identifier for the password reset code.
	/// </summary>
	public Guid Id { get; set; }

	/// <summary>
	/// Gets or sets the email address associated with the password reset code.
	/// </summary>
	public string Email { get; set; }

	/// <summary>
	/// Gets or sets the code used for password reset.
	/// </summary>
	public string Code { get; set; }

	/// <summary>
	/// Gets or sets the date and time when the password reset code was created.
	/// </summary>
	public DateTime CreatedAt { get; set; }
}
