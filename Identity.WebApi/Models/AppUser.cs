using Microsoft.AspNetCore.Identity;

namespace Identity.WebApi.Models
{
	/// <summary>
	/// Represents an application user.
	/// </summary>
	public class AppUser : IdentityUser<Guid>
	{
		/// <summary>
		/// Gets or sets the first name of the user.
		/// </summary>
		public string FirstName { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the last name of the user.
		/// </summary>
		public string LastName { get; set; } = string.Empty;
	}
}
