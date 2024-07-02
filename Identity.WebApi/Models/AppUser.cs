using Microsoft.AspNetCore.Identity;
using System;

namespace Identity.WebApi.Models
{
	/// <summary>
	/// Represents the application user.
	/// </summary>
	public class AppUser : IdentityUser<Guid>
	{
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string Address { get; set; } = string.Empty;
		public DateTime PasswordLastChanged { get; set; } = DateTime.UtcNow;
	}
}
